using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class Loader
{
	public enum Scene
	{
		GameScene,
		MainMenu,
		LoadingScene,
		LobbyScene,
		SelectCharacterScene
	}

	private static Scene loadingScene;

	public static void Load(Scene loadingScene)
	{
		Loader.loadingScene = loadingScene;
		SceneManager.LoadScene(Scene.LoadingScene.ToString());
	}

	public static void LoadNetwork(Scene loadingScene)
	{
		NetworkManager.Singleton.SceneManager.LoadScene(loadingScene.ToString(), LoadSceneMode.Single);
	}

	public static void LoadCallBackScene()
	{
		SceneManager.LoadScene(loadingScene.ToString());
	}
}
