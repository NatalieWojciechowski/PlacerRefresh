using UnityEngine.SceneManagement;

internal interface I_RefreshOnSceneChange
{    
    public abstract void OnSceneChange(Scene current, Scene next);
    public abstract void ReInit();
}