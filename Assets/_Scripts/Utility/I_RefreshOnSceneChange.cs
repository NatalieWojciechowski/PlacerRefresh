using UnityEngine.SceneManagement;

public interface I_RefreshOnSceneChange
{    
    public abstract void OnSceneChange(Scene current, Scene next);
    public abstract void ReInit();
    public abstract void OnSceneLoad(Scene current, LoadSceneMode loadSceneMode);
}