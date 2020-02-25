using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssemblyManager : MonoBehaviour {

    public Transform CameraTrans;

    public Database DatabaseRef;
    public HudController HudControllerRef;
    public RemarksManager RemarksManagerRef;
    public Material PartMat;

    private int CurrentMove;
    private int CurrentPart;

    public Vector3 InitialCamPos;
    public Quaternion InitialCamRot;

    private Move[] _moves;
    private bool _doPlayAnimation;

    // Use this for initialization
    void Start() {

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
        
        ResetToCurrentMove();
        UpdateHud();
        StartCoroutine(MoveCameraToCurrentMove(false));
    }

    private void ButtonPressed(HudController.ButtonType button) {

        StopAllCoroutines();

        switch (button) {
            case HudController.ButtonType.StartPLaying:
                _doPlayAnimation = true;
                UpdateHud();
                StartCoroutine(PlayCurrentMoveExternalAnimation());
                break;
            case HudController.ButtonType.StopPLaying:
                //StartCoroutine(PlayFullPart(false));
                _doPlayAnimation = false;
                ResetToCurrentMove();
                UpdateHud();
                StartCoroutine(MoveCameraToCurrentMove(false));

                break;
            case HudController.ButtonType.OneBack:
                if (CurrentMove > -1) {
                    CurrentMove--;
                }

                ResetToCurrentMove();
                UpdateHud();
                StartCoroutine(MoveCameraToCurrentMove(false));

                break;
            case HudController.ButtonType.OneForward:
                if (CurrentMove < _moves.Length - 1) {
                    CurrentMove++;
                }
                ResetToCurrentMove();
                UpdateHud();
                StartCoroutine(MoveCameraToCurrentMove(false));

                break;
            case HudController.ButtonType.FullBack:
                CurrentMove = -1;
                ResetToCurrentMove();
                UpdateHud();
                StartCoroutine(MoveCameraToCurrentMove(false));

                break;
        }
    }

    void Update() {
        /*if (Input.GetKeyDown(KeyCode.UpArrow)) {
            CurrentMove++;
            StartCoroutine(PlayMove(true));
        }else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            CurrentMove--;
            StartCoroutine(PlayMove(false));
        }*/
    }



    private IEnumerator PlayCurrentMoveExternalAnimation() {



        yield return StartCoroutine(MoveCameraToCurrentMove(CurrentMove > -1));

        while (_doPlayAnimation) {

            ResetToCurrentMove();


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
            
            float moveTime = 1;

            Vector3 startPos = CameraTrans.position;
            Quaternion startRot = CameraTrans.rotation;

            float start = Time.time;
            while (Time.time < start + moveTime) {

                float prog = Mathf.Clamp((Time.time - start) / moveTime, 0, 1);
                CameraTrans.localPosition = Vector3.Lerp(startPos, (CurrentMove > -1) ? (m.CameraPos) : (InitialCamPos), prog);
                CameraTrans.localRotation = Quaternion.Slerp(startRot, (CurrentMove > -1) ? (m.CameraRot) : (InitialCamRot), prog);

                yield return null;
            }
        }
        

        CameraTrans.localPosition = (CurrentMove > -1) ? (m.CameraPos) : (InitialCamPos);
        CameraTrans.localRotation = (CurrentMove > -1) ? (m.CameraRot) : (InitialCamRot);
    }

    private void ResetToCurrentMove() {

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
        InitialCamPos = EditorWindow.GetWindow<SceneView>().camera.transform.position;
        InitialCamRot = EditorWindow.GetWindow<SceneView>().camera.transform.rotation;
    }

    internal void ApplyMat() {
        DatabaseRef.ApplyMat(PartMat);
    }

    

#endif
}
