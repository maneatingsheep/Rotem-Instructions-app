
public interface EditorControlled {
    void CapturePartRotation();

    void ApplyPartRotation();

    void CapturePartFocus();

    void ApplyPartFocus();

    void CaptureTransformsbyNames(int index, bool addToExistingList);

    void CaptureRemarkTargetsNames(int index, bool addToExistingList);


}
