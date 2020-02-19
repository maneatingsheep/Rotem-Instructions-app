using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssemblyManager : MonoBehaviour {

    public Transform CameraTrans;

    public Database DatabaseRef;
    public HudController HudControllerRef;
    public Material PartMat;

    private int CurrentMove = -1;

    public Vector3 InitialCamPos;
    public Quaternion InitialCamRot;

    private Move[] _moves;
    // Use this for initialization
    void Start () {

        _moves = DatabaseRef.Init();
        HudControllerRef.SetUIState(HudController.UIState.First);
        HudControllerRef.ButtonPressedCallback += ButtonPressed;
        StartCoroutine(PlayCameraRepositioning(false));
    }

    private void ButtonPressed(HudController.ButtonType button) {

        StopAllCoroutines();

        switch (button) {
            case HudController.ButtonType.PlayOne:
                StartCoroutine(PlayMove(true));
                break;
            case HudController.ButtonType.PlayAll:
                StartCoroutine(PlayMove(false));
                break;
            case HudController.ButtonType.OneBack:
                if (CurrentMove > -1) {
                    CurrentMove--;
                }
                
                ResetMasterMove();
                StartCoroutine(PlayCameraRepositioning(false));

                break;
            case HudController.ButtonType.OneForward:
                if (CurrentMove < _moves.Length - 1) {
                    CurrentMove++;
                }
                ResetMasterMove();
                StartCoroutine(PlayCameraRepositioning(false));

                break;
            case HudController.ButtonType.FullBack:
                CurrentMove = -1;
                ResetMasterMove();
                StartCoroutine(PlayCameraRepositioning(false));

                break;
        }
    }

    void Update () {
		/*if (Input.GetKeyDown(KeyCode.UpArrow)) {
            CurrentMove++;
            StartCoroutine(PlayMove(true));
        }else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            CurrentMove--;
            StartCoroutine(PlayMove(false));
        }*/
    }



    private IEnumerator PlayMove(bool playOne) {
        
        
        bool doPlay = true;

        while (CurrentMove < _moves.Length && doPlay) {
            
            doPlay = !playOne;

            yield return StartCoroutine(PlayCameraRepositioning(CurrentMove > -1));


            ResetMasterMove();


            if (CurrentMove > -1) {


                yield return new WaitForSeconds(1);

                yield return StartCoroutine(PlayMasterMoveAnimation());
            } else {

                yield return new WaitForSeconds(2);
            }

            GoToMasterMoveEnd();

            CurrentMove++;
        }

        CurrentMove--;

    }

    private IEnumerator PlayCameraRepositioning(bool doAnimate) {

        HudControllerRef.SetUIState((CurrentMove == -1) ? HudController.UIState.First : ((CurrentMove < _moves.Length - 1) ? HudController.UIState.Middle : HudController.UIState.Last));

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

    private void ResetMasterMove() {
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

    private void GoToMasterMoveEnd() {
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

    private IEnumerator PlayMasterMoveAnimation() {
        Move m = _moves[CurrentMove];
        if (m.Submoves != null) {
            for (int j = 0; j < m.Submoves.Length; j++) {
                yield return StartCoroutine(PlaySingleMoveAnimation(m.Submoves[j]));
                if (j < m.Submoves.Length - 1) {
                    yield return new WaitForSeconds(1);
                }
            }
        } else {
            yield return StartCoroutine(PlaySingleMoveAnimation(m));
        }
    }

    private IEnumerator PlaySingleMoveAnimation(Move m) {
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
