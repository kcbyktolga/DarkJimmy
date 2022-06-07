namespace DarkJimmy.UI
{
    public class Loading : Splash
    {
        public override void Start()
        {
            StartCoroutine(Skip(Menus.Lobby.ToString()));
        }
    }
}
