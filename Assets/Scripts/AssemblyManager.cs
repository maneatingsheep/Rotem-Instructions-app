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
    public Material PartMat;

    public Transform AllPartsRoot;
    public Transform AllPartsRootRef;

    private int CurrentMove;
    private int CurrentPart;

    /*public Vector3 InitialCamPos;
    public Quaternion InitialCamRot;*/
    public Quaternion InitialRotation;

    private Move[] _moves;
    private bool _doPlayAnimation;

    private Vector2 _startTouch;
    private bool _isTouching = false;

    // Use this for initialization
    void Start() {

        RemarksManagerRef.Init();
        DatabaseRef.Init();
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
        
        ResetToCurrentMove(true);
        UpdateHud();
        StartCoroutine(MoveCameraToCurrentMove(false));
    }

    private void ButtonPressed(HudController.ButtonType button) {

        StopAllCoroutines();

        switch (button) {
            case HudController.ButtonType.StartPLaying:
                _doPlayAnimation = true;
                StartCoroutine(PlayCurrentMoveExternalAnimation());
                UpdateHud();
                break;
            case HudController.ButtonType.StopPLaying:
                //StartCoroutine(PlayFullPart(false));
                _doPlayAnimation = false;
                ResetToCurrentMove(false);
                UpdateHud();
                StartCoroutine(MoveCameraToCurrentMove(false));

                break;
            case HudController.ButtonType.OneBack:
                if (CurrentMove > -1) {
                    CurrentMove--;
                }
                _doPlayAnimation = false;
                ResetToCurrentMove(true);
                UpdateHud();
                StartCoroutine(MoveCameraToCurrentMove(false));

                break;
            case HudController.ButtonType.OneForward:
                if (CurrentMove < _moves.Length - 1) {
                    CurrentMove++;
                }
                _doPlayAnimation = false;
                ResetToCurrentMove(true);
                UpdateHud();
                StartCoroutine(MoveCameraToCurrentMove(false));

                break;
            case HudController.ButtonType.FullBack:
                CurrentMove = -1;
                ResetToCurrentMove(true);
                UpdateHud();
                StartCoroutine(MoveCameraToCurrentMove(false));

                break;
        }
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            _isTouching = true;
            _startTouch = Input.mousePosition;
        } else if (Input.GetMouseButtonUp(0)) {
            _isTouching = false;
        }

        if (_isTouching) {

            Vector2 _curentPos = Input.mousePosition;
            Vector2 dir = _curentPos - _startTouch;

            Vector3 initialAxis = AllPartsRoot.transform.position - ActiveCamera.ScreenToWorldPoint(new Vector3(_startTouch.x, _startTouch.y, 30f));
            Vector3 finalAxis = AllPartsRoot.transform.position - ActiveCamera.ScreenToWorldPoint(new Vector3(_curentPos.x, _curentPos.y, 30f));
            Debug.DrawLine(AllPartsRoot.transform.position - finalAxis, AllPartsRoot.transform.position);

            Quaternion rot = Quaternion.FromToRotation(initialAxis, finalAxis);
            rot *= rot;
            rot *= rot;
            rot *= rot;
           
            AllPartsRoot.transform.rotation *= rot;

            //double angle = Math.Atan2(dir.y, dir.x);

            //dir = Quaternion.Euler(0f, 0f, -90) * dir;

            //AllPartsRoot.transform.Rotate(dir, dir.magnitude);

            _startTouch = _curentPos;
        }
    }



    private IEnumerator PlayCurrentMoveExternalAnimation() {



        yield return StartCoroutine(MoveCameraToCurrentMove(CurrentMove > -1));

        while (_doPlayAnimation) {

            ResetToCurrentMove(false);


            if (CurrentMove > -1) {


                yield return new WaitForSeconds(1);

                yield return StartCoroutine(PlayCurrentMoveInternalAnimation());

                yield return new WaitForSeconds(1);
            }

            GoToCurrentMoveEnd();

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

        HudControllerRef.SetUIState(usState, _doPlayAnimation);
    }

    private IEnumerator MoveCameraToCurrentMove(bool doAnimate) {

        

        Move m = null;
        if (CurrentMove > -1) {
            m = (_moves[CurrentMove].Submoves == null) ? _moves[CurrentMove] : _moves[CurrentMove].Submoves[0];
        }

        if (doAnimate) {

            iTween.RotateTo(AllPartsRoot.gameObject, m.ViewRot.eulerAngles, 1f);
            yield return new WaitForSeconds(1.1f);

            /*float moveTime = 1;

            Vector3 startPos = CameraTrans.position;
            Quaternion startRot = CameraTrans.rotation;

            float start = Time.time;
            while (Time.time < start + moveTime) {

                float prog = Mathf.Clamp((Time.time - start) / moveTime, 0, 1);
                AllPartsRoot.transform.rotation = Quaternion.Slerp(startRot, (CurrentMove > -1) ? (m.ViewRot) : (InitialRotation), prog);


                //CameraTrans.localPosition = Vector3.Lerp(startPos, (CurrentMove > -1) ? (m.CameraPos) : (InitialCamPos), prog);
                //CameraTrans.localRotation = Quaternion.Slerp(startRot, (CurrentMove > -1) ? (m.CameraRot) : (InitialCamRot), prog);

                yield return null;
            }*/
        }

        AllPartsRoot.transform.rotation = (CurrentMove > -1) ? (m.ViewRot) : (InitialRotation);

        //CameraTrans.localPosition = (CurrentMove > -1) ? (m.CameraPos) : (InitialCamPos);
        //CameraTrans.localRotation = (CurrentMove > -1) ? (m.CameraRot) : (InitialCamRot);
    }

    private void ResetToCurrentMove(bool doResetFreeRotation) {

        if (doResetFreeRotation) {
            AllPartsRoot.transform.rotation = Quaternion.identity;
        }

        if (CurrentMove != -1) {

            if (_moves[CurrentMove].Submoves != null) {
                RemarksManagerRef.SetRemarks(_moves[CurrentMove].Submoves[0]);
            } else {
            
                RemarksManagerRef.SetRemarks(_moves[CurrentMove]);
            } 
        } else {
            RemarksManagerRef.SetRemarks(null);
        }
        

        for (int i = 0; i < _moves.Length; i++) {
            Move m = _moves[i];
            if (m.Submoves != null) {
                for (int j = 0; j < m.Submoves.Length; j++) {
                    ResetSingleMove(m.Submoves[j], i);
                }
            } else {
                ResetSingleMove(m, i);
            }
        }

    }

    private void ResetSingleMove(Move m, int movenum) {
        for (int j = 0; j < m.Transforms.Length; j++) {
            Transform t = m.Transforms[j];
            if (movenum > CurrentMove && (CurrentMove != -1)) {
                t.gameObject.SetActive(false);
            } else {
                t.gameObject.SetActive(true);
                if (CurrentMove == -1 || movenum < CurrentMove) {
                    t.localPosition = m.Final.Pos[j];
                    t.localRotation = m.Final.Rot[j];
                } else {
                    t.localPosition = m.Initital.Pos[j];
                    t.localRotation = m.Initital.Rot[j];
                }
            }
        }
    }

    private void GoToCurrentMoveEnd() {
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
        
    }

    private void GoToSingleMoveEnd(Move m) {
        for (int i = 0; i < m.Transforms.Length; i++) {
            Transform t = m.Transforms[i];
            t.localPosition = m.Final.Pos[i];
            t.localRotation = m.Final.Rot[i];
        }
    }

    private IEnumerator PlayCurrentMoveInternalAnimation() {
        Move m = _moves[CurrentMove];
        if (m.Submoves != null) {
            for (int j = 0; j < m.Submoves.Length; j++) {
                yield return StartCoroutine(PlaySingleSubMoveAnimation(m.Submoves[j]));
                if (j < m.Submoves.Length - 1) {
                    yield return new WaitForSeconds(1);
                }
            }
        } else {
            yield return StartCoroutine(PlaySingleSubMoveAnimation(m));
        }
    }

    private IEnumerator PlaySingleSubMoveAnimation(Move m) {
        float moveTime = 1;

        float start = Time.time;
        while (Time.time < start + moveTime) {
            for (int i = 0; i < m.Transforms.Length; i++) {
                Transform t = m.Transforms[i];
                float prog = Mathf.Clamp((Time.time - start) / moveTime, 0, 1);
                t.localPosition = Vector3.Lerp(m.Initital.Pos[i], m.Final.Pos[i], prog);
                t.localRotation = Quaternion.Slerp(m.Initital.Rot[i], m.Final.Rot[i], prog);
            }

            yield return null;
        }
    }
#if UNITY_EDITOR
    internal void CaptureCamera() {
        
        InitialRotation = AllPartsRootRef.transform.rotation;

        /*InitialCamPos = EditorWindow.GetWindow<SceneView>().camera.transform.position;
        InitialCamRot = EditorWindow.GetWindow<SceneView>().camera.transform.rotation;*/
    }

    internal void ApplyMat() {
        DatabaseRef.ApplyMat(PartMat);
    }

    

#endif
}
