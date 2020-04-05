using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssemblyManager : MonoBehaviour {

    public Transform CameraTrans;
    public Camera ActiveCamera;

    public Database DatabaseRef;
    public HudController HudControllerRef;
    public RemarksManager RemarksManagerRef;

    public Transform AllPartsRoot;
    public Transform RotatorCont;


    public Material PartMaterial;
    public Color OpaqueColor;
    public Color TransparentColor;

    private int CurrentMove;
    private int CurrentPart;

    bool _isAnimating = false;

    private Move[] _moves;
    
    private Vector3 _startTouch;
    private bool _isTouching = false;
    private Coroutine _animCR;

    public float ANIMATION_TIME;
    public float TRANSITION_TIME;
    public float REST_TIME;

    public float CamDistFromCenter;

    void Start() {

        ActiveCamera.transform.position = new Vector3(0, 0, -CamDistFromCenter);

        GameObject fp = GameObject.Find("full part");
        if (fp != null) {
            fp.SetActive(false);
        }

        fp = GameObject.Find("full unused");
        if (fp != null) {
            fp.SetActive(false);
        }
        
    
        fp = GameObject.Find("full used");
        if (fp != null) {
            fp.SetActive(false);
        }

        RemarksManagerRef.Init();
        DatabaseRef.Init(PartMaterial);
        HudControllerRef.Init(DatabaseRef.GetChapters());
        

        HudControllerRef.ButtonPressedCallback += ButtonPressed;
        HudControllerRef.ChapterPressedCallback += LoadChapter; ;


        LoadChapter(0);
    }

    private void LoadChapter(int chapter)
    {
        StopAllCoroutines();

        RemarksManagerRef.OpenHideRemarks(true);

        CurrentMove = -1;
        CurrentPart = chapter;

        _moves = DatabaseRef.BuildMoves(CurrentPart);

        StartCoroutine(ResetToCurrentMove(false));
        StartCoroutine(MoveCameraToCurrentMove(false));

        UpdateHud();
    }

    private void ButtonPressed(HudController.ButtonType button) {

        StopAnimation();
        switch (button) {
            case HudController.ButtonType.StartPLaying:
                StartAnimation(false);
                break;
            case HudController.ButtonType.StopPLaying:
                StopAnimation();
                break;
            case HudController.ButtonType.OneBack:
                if (CurrentMove > -1) {
                    CurrentMove--;
                }

                StartAnimation(true);

                break;
            case HudController.ButtonType.OneForward:
                if (CurrentMove < _moves.Length - 1) {
                    CurrentMove++;
                }

                StartAnimation(true);
                break;
            case HudController.ButtonType.FullBack:
                
                CurrentMove = -1;
                StartCoroutine(ResetToCurrentMove(true));
                StartCoroutine(MoveCameraToCurrentMove(true));

                

                break;
        }

        UpdateHud();
    }

    private void StartAnimation(bool showTransition) {
        _isAnimating = true;
        if (showTransition) {
            StartCoroutine(MoveCameraToCurrentMove(true));
        }
        _animCR =  StartCoroutine(PlayCurrentMoveContiniousAnimation(showTransition));
    }

    private void StopAnimation() {
        if (_animCR != null) {
            //StopCoroutine(_animCR);
            StopAllCoroutines();
            _animCR = null;
        }
        ResetToCurrentMove(false);
        _isAnimating = false;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            _isTouching = true;
            
        } else if (Input.GetMouseButtonUp(0)) {
            _isTouching = false;
        }

        if (_isTouching) {
            Ray r = ActiveCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            RaycastHit rh;
            Vector3 _curentPos;

            if (Physics.Raycast(r, out rh)) {
                _curentPos = rh.point;
                Debug.DrawLine(Vector3.zero, _startTouch);
                Debug.DrawLine(Vector3.zero, _curentPos);

                /*Quaternion fromrot = Quaternion.LookRotation(_startTouch, Vector3.up);
                Quaternion torot = Quaternion.LookRotation(_curentPos, Vector3.up);*/

                RotatorCont.transform.localRotation = Quaternion.FromToRotation(_startTouch, _curentPos) * RotatorCont.transform.localRotation;

                _startTouch = _curentPos;


            } else {
                _isTouching = false;

            }

            
        }
    }

    private void OnMouseDown() {
        Ray r = ActiveCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        RaycastHit rh;
        if (Physics.Raycast(r, out rh)) {
            _isTouching = true;
            _startTouch = rh.point;
        }else {
            _isTouching = false;
        }
    }

    private void OnMouseExit() {
        _isTouching = false;
    }

    private void OnMouseUp() {
        _isTouching = false;
    }

    private IEnumerator PlayCurrentMoveContiniousAnimation(bool showTransition) {


        yield return StartCoroutine(ResetToCurrentMove(showTransition));

        while (true) {


            if (CurrentMove > -1) {

                yield return ResetToCurrentMove(false);
                yield return new WaitForSeconds(REST_TIME);


                yield return StartCoroutine(PlayCurrentMovePartAnimation());

                yield return new WaitForSeconds(REST_TIME);
            }

            //GoToCurrentMoveEnd();

        }


    }

    private void UpdateHud()
    {
        HudController.UIState usState = HudController.UIState.Master;

        if (CurrentMove == 0)
        {
            usState = HudController.UIState.First;

        }
        else if (CurrentMove == _moves.Length - 1)
        {
            usState = HudController.UIState.Last;
        }
        else if (CurrentMove > 0)
        {
            usState = HudController.UIState.Middle;
        }

        HudControllerRef.SetUIState(usState, _isAnimating);
    }

    private IEnumerator MoveCameraToCurrentMove(bool doAnimate) {


        Quaternion rot = DatabaseRef.Part.InitialRotation;
        Vector3 focalRelativePoint = Vector3.zero;
        float camDist = DatabaseRef.Part.InitialCamDist;


        if (CurrentMove > -1) {
            Move m = (_moves[CurrentMove].Submoves == null) ? _moves[CurrentMove] : _moves[CurrentMove].Submoves[0];
            rot = m.ViewRot;

            focalRelativePoint = m.ViewFocusPoint;
            camDist = m.ViewCamDistance;
        }


        if (doAnimate) {

            AllPartsRoot.parent = AllPartsRoot.parent.parent;
            RotatorCont.transform.position = AllPartsRoot.position + focalRelativePoint;
            AllPartsRoot.parent = RotatorCont;

            iTween.MoveTo(RotatorCont.gameObject, Vector3.zero, TRANSITION_TIME);

            iTween.MoveTo(RotatorCont.gameObject, new Vector3(0, 0, camDist - 10), TRANSITION_TIME);

            iTween.RotateTo(RotatorCont.gameObject, rot.eulerAngles, TRANSITION_TIME);
            yield return new WaitForSeconds(TRANSITION_TIME + REST_TIME);

        } else {
            RotatorCont.position = Vector3.zero;
            RotatorCont.transform.position = new Vector3(0, 0, camDist - CamDistFromCenter);
            RotatorCont.transform.rotation = rot;
        }


        //CameraTrans.localPosition = (CurrentMove > -1) ? (m.CameraPos) : (InitialCamPos);
        //CameraTrans.localRotation = (CurrentMove > -1) ? (m.CameraRot) : (InitialCamRot);
    }

    private IEnumerator ResetToCurrentMove(bool showTransition) {
        
        //remarks
        if (CurrentMove != -1) {
            if (_moves[CurrentMove].Submoves != null) {
                RemarksManagerRef.SetRemarks(_moves[CurrentMove].Submoves[0]);
            } else {
            
                RemarksManagerRef.SetRemarks(_moves[CurrentMove]);
            } 
        } else {
            RemarksManagerRef.SetRemarks(null);
        }
        
        //reset moves
        for (int i = 0; i < _moves.Length; i++) {
            Move m = _moves[i];
            if (m.Submoves != null) {
                for (int j = 0; j < m.Submoves.Length; j++) {
                    ResetSingleMove(m.Submoves[j], i, showTransition);
                }
            } else {
                ResetSingleMove(m, i, showTransition);
            }
        }

        //assembly visibility
        for (int i = 0; i < DatabaseRef.Part.Assemblies.Length; i++) {
            if (CurrentMove == -1 || DatabaseRef.Part.Assemblies[i].Name == _moves[CurrentMove].Assembly || Array.Exists<string>(_moves[CurrentMove].SupportingAssemblies, (s) => (DatabaseRef.Part.Assemblies[i].Name == s))) {
                Transform assemblyTr = AllPartsRoot.Find(DatabaseRef.Part.Assemblies[i].Name);
                assemblyTr.gameObject.SetActive(true);
                for (int j = 0; j < DatabaseRef.Part.Assemblies[i].StaticParts.Length; j++) {
                    assemblyTr.Find(DatabaseRef.Part.Assemblies[i].StaticParts[j]).gameObject.SetActive(true);
                }
            } else {
                AllPartsRoot.Find(DatabaseRef.Part.Assemblies[i].Name).gameObject.SetActive(false);
            }
        }

        if (showTransition) {
            yield return new WaitForSeconds(TRANSITION_TIME);
        }
    }

    private void ResetSingleMove(Move m, int movenum, bool doFade) {

        bool isSupportingAssembly = false;
        
        if (CurrentMove > -1) {
            isSupportingAssembly = Array.Exists<string>(_moves[CurrentMove].SupportingAssemblies, (s) => s == m.Assembly);
        }



        for (int j = 0; j < m.Transforms.Length; j++) {

            

            Transform t = m.Transforms[j];
            bool isVisible = false;
            if (isSupportingAssembly || movenum <= CurrentMove || CurrentMove == -1) {
                
                isVisible = true;
                
                if (CurrentMove == -1 || isSupportingAssembly || movenum < CurrentMove) {
                    t.localPosition = m.Final.Pos[j];
                    t.localRotation = m.Final.Rot[j];
                } else {
                    t.localPosition = m.Initital.Pos[j];
                    t.localRotation = m.Initital.Rot[j];
                }
            }

            

            if (t.childCount > 0) {
                //dont turn it off here. assemblies turned on and off elswere
                /*if (isVisible) {
                    for (int i = 0; i < t.childCount; i++) {
                        SetVisibility(t.GetChild(i), isVisible, doFade);
                    }

                }*/
            } else {
                SetVisibility(t, isVisible, doFade);
            }

        }
    }

    private void SetVisibility(Transform t, bool isVisible, bool doFade) {
        Color c = (isVisible) ? OpaqueColor : TransparentColor;
        t.gameObject.SetActive(isVisible);

        /*if (doFade) {
            iTween.ColorTo(t.gameObject, c, TRANSITION_TIME);
            
        } else {
            t.GetComponent<MeshRenderer>().material.SetColor("_MainTexture", c);
        }*/

    }

    /*private void GoToCurrentMoveEnd() {
        if (CurrentMove > -1) {
            Move m = _moves[CurrentMove];
            if (m.Submoves != null) {
                for (int j = 0; j < m.Submoves.Length; j++) {
                    GoToSingleMoveEnd(m.Submoves[j]);
                }
            } else {
                GoToSingleMoveEnd(m);
            }
        } else {
            foreach (Move m in _moves) {
                if (m.Submoves != null) {
                    for (int i = 0; i < m.Submoves.Length; i++) {
                        for (int j = 0; j < m.Submoves[i].Transforms.Length; j++) {
                            Transform t = m.Submoves[i].Transforms[j];
                            t.gameObject.SetActive(false);
                        }
                    }

                } else {
                    for (int j = 0; j < m.Transforms.Length; j++) {
                        Transform t = m.Transforms[j];
                        t.gameObject.SetActive(false);
                    }
                }
            }
            
        }
        
    }*/

    /*private void GoToSingleMoveEnd(Move m) {
        for (int i = 0; i < m.Transforms.Length; i++) {
            Transform t = m.Transforms[i];
            t.localPosition = m.Final.Pos[i];
            t.localRotation = m.Final.Rot[i];
        }
    }*/

    private IEnumerator PlayCurrentMovePartAnimation() {
        Move m = _moves[CurrentMove];
        if (m.Submoves != null) {
            for (int j = 0; j < m.Submoves.Length; j++) {
                yield return StartCoroutine(PlaySingleSubMoveAnimation(m.Submoves[j]));
                if (j < m.Submoves.Length - 1) {
                    yield return new WaitForSeconds(REST_TIME);
                }
            }
        } else {
            yield return StartCoroutine(PlaySingleSubMoveAnimation(m));
        }
    }

    private IEnumerator PlaySingleSubMoveAnimation(Move m) {
        
        float start = Time.time;
        while (Time.time < start + ANIMATION_TIME) {
            for (int i = 0; i < m.Transforms.Length; i++) {
                Transform t = m.Transforms[i];
                float prog = Mathf.Clamp((Time.time - start) / ANIMATION_TIME, 0, 1);
                t.localPosition = Vector3.Lerp(m.Initital.Pos[i], m.Final.Pos[i], prog);
                t.localRotation = Quaternion.Slerp(m.Initital.Rot[i], m.Final.Rot[i], prog);
            }

            yield return null;
        }

        for (int i = 0; i < m.Transforms.Length; i++) {
            Transform t = m.Transforms[i];
            t.localPosition = m.Final.Pos[i];
            t.localRotation = m.Final.Rot[i];
        }

    }
}
