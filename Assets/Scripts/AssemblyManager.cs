using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssemblyManager : MonoBehaviour {

    public Transform CameraTrans;
    public Camera ActiveCamera;

    public Database DatabaseRef;
    public NavController NavControllerRef;
    public MenuManager MenuManagerRef;
    public RemarksManager RemarksManagerRef;
    public InventoryManager InventoryManagerRef;

    public Transform AllPartsRoot;
    public Transform RotatorCont;


    public Material PartMaterial;
    public Color OpaqueColor;
    public Color TransparentColor;

    private int CurrentMove;
    private int CurrentPart;

    bool _isAnimating = false;
    
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
        

        NavControllerRef.ButtonPressedCallback += ButtonPressed;
        MenuManagerRef.StepPressedCallback += GotoMove;


        LoadChapter(0);
    }


    private void LoadChapter(int chapter)
    {
        StopAllCoroutines();


        CurrentMove = -1;
        CurrentPart = chapter;

        DatabaseRef.BuildMoves(CurrentPart);

        StartCoroutine(ResetToCurrentMove(false));
        StartCoroutine(MoveCameraToCurrentMove(false));

        MenuManagerRef.Init(DatabaseRef.Moves);
        UpdateHud();
    }

    private void ButtonPressed(NavController.ButtonType button) {

        
        switch (button) {
            case NavController.ButtonType.StartPLaying:
                
                StartAnimation(false);
                break;
            case NavController.ButtonType.StopPLaying:
                StopAnimation();
                break;
            case NavController.ButtonType.OneBack:
                if (CurrentMove > -1) {
                    GotoMove(CurrentMove-1);
                }
                

                break;
            case NavController.ButtonType.OneForward:
                if (CurrentMove < DatabaseRef.Moves.Length - 1) {
                    GotoMove(CurrentMove + 1);
                }
                break;
            case NavController.ButtonType.FullBack:
                StopAnimation();
                CurrentMove = -1;
                StartCoroutine(ResetToCurrentMove(true));
                StartCoroutine(MoveCameraToCurrentMove(true));


                break;
        }

        UpdateHud();
    }

    private void GotoMove(int moveNum) {
        if (!MenuManagerRef.isRemarksOpen) {
            MenuManagerRef.RemarksToggled();
        }
        StopAnimation();
        CurrentMove = moveNum;
        StartAnimation(true);
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
        NavController.UIState usState = NavController.UIState.Master;

        if (CurrentMove == 0)
        {
            usState = NavController.UIState.First;

        }
        else if (CurrentMove == DatabaseRef.Moves.Length - 1)
        {
            usState = NavController.UIState.Last;
        }
        else if (CurrentMove > 0)
        {
            usState = NavController.UIState.Middle;
        }

        NavControllerRef.SetUIState(usState, _isAnimating, CurrentMove, DatabaseRef.Moves.Length);
        MenuManagerRef.UpdateToStepNum(CurrentMove);
    }

    private IEnumerator MoveCameraToCurrentMove(bool doAnimate) {


        Quaternion rot = DatabaseRef.PartData.InitialRotation;
        Vector3 focalRelativePoint = Vector3.zero;
        float camDist = DatabaseRef.PartData.InitialCamDist;


        if (CurrentMove > -1) {
            rot = DatabaseRef.Moves[CurrentMove].ViewRot;

            focalRelativePoint = DatabaseRef.Moves[CurrentMove].ViewFocusPoint;
            camDist = DatabaseRef.Moves[CurrentMove].ViewCamDistance;
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


       
    }

    private IEnumerator ResetToCurrentMove(bool showTransition) {


        if (CurrentMove > -1) {
            InventoryManagerRef.UpdateInventory(DatabaseRef.Moves[CurrentMove]);
        } else {
            InventoryManagerRef.UpdateInventory(DatabaseRef.Moves);
        }

        //remarks
        if (CurrentMove != -1) {
                       
            RemarksManagerRef.SetRemarks(DatabaseRef.Moves[CurrentMove]);
            
        } else {
            RemarksManagerRef.SetRemarks(null);
        }


        //reset moves
        for (int i = 0; i < DatabaseRef.Moves.Length; i++) {
            ResetSingleMove(DatabaseRef.Moves[i], i, showTransition);
        }

        //assembly visibility
        for (int i = 0; i < DatabaseRef.PartData.Assemblies.Length; i++) {
            if (CurrentMove == -1 || DatabaseRef.PartData.Assemblies[i].ElementName == DatabaseRef.Moves[CurrentMove].AssemblyName || Array.Exists<string>(DatabaseRef.Moves[CurrentMove].SupportingAssemblies, (s) => (DatabaseRef.PartData.Assemblies[i].ElementName == s))) {
                Transform assemblyTr = AllPartsRoot.Find(DatabaseRef.PartData.Assemblies[i].ElementName);
                assemblyTr.gameObject.SetActive(true);
                for (int j = 0; j < DatabaseRef.PartData.Assemblies[i].Transforms.Length; j++) {
                    DatabaseRef.PartData.Assemblies[i].Transforms[j].gameObject.SetActive(true);
                }
            } else {
                AllPartsRoot.Find(DatabaseRef.PartData.Assemblies[i].ElementName).gameObject.SetActive(false);
            }
        }

        if (showTransition) {
            yield return new WaitForSeconds(TRANSITION_TIME);
        }
    }

    private void ResetSingleMove(Move m, int movenum, bool doFade) {

        bool isSupportingAssembly = false;
        
        if (CurrentMove > -1) {
            isSupportingAssembly = Array.Exists<string>(DatabaseRef.Moves[CurrentMove].SupportingAssemblies, (s) => s == m.AssemblyName);
        }


        for (int i = 0; i < m.Transformations.Length; i++) {
            for (int j = 0; j < m.Transformations[i].Elements.Transforms.Length; j++) {

                Transform t = m.Transformations[i].Elements.Transforms[j];
                bool isVisible = false;
                if (isSupportingAssembly || movenum <= CurrentMove || CurrentMove == -1) {

                    isVisible = true;

                    if (CurrentMove == -1 || isSupportingAssembly || movenum < CurrentMove) {
                        t.localPosition = m.Transformations[i].Final.Pos[j];
                        t.localRotation = m.Transformations[i].Final.Rot[j];
                    } else {
                        t.localPosition = m.Transformations[i].Initial.Pos[j];
                        t.localRotation = m.Transformations[i].Initial.Rot[j];
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
        Move m = DatabaseRef.Moves[CurrentMove];

        for (int j = 0; j < m.Transformations.Length; j++) {
            yield return StartCoroutine(PlaySingleSubMoveAnimation(m.Transformations[j]));
            if (j < m.Transformations.Length - 1) {
                yield return new WaitForSeconds(REST_TIME);
            }
        }
    }

    private IEnumerator PlaySingleSubMoveAnimation(Transformation tr) {
        
        float start = Time.time;
        while (Time.time < start + ANIMATION_TIME) {
            for (int i = 0; i < tr.Elements.Transforms.Length; i++) {
                Transform t = tr.Elements.Transforms[i];
                float prog = Mathf.Clamp((Time.time - start) / ANIMATION_TIME, 0, 1);
                t.localPosition = Vector3.Lerp(tr.Initial.Pos[i], tr.Final.Pos[i], prog);
                t.localRotation = Quaternion.Slerp(tr.Initial.Rot[i], tr.Final.Rot[i], prog);
            }

            yield return null;
        }

        for (int i = 0; i < tr.Elements.Transforms.Length; i++) {
            Transform t = tr.Elements.Transforms[i];

            t.localPosition = tr.Final.Pos[i];
            t.localRotation = tr.Final.Rot[i];
        }

    }
}
