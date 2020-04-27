
public interface EditorControlled {


#if UNITY_EDITOR
    void CapturePartView();

    void ApplyPartView();


    void CaptureTransformsbyNames(int index, bool addToExistingList);

    void CaptureRemarkTargetsNames(int index, bool addToExistingList);
# endif

}
