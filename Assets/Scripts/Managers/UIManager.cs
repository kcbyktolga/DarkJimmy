using System.Collections.Generic;
using UnityEngine;
using DarkJimmy.UI;
using UnityEngine.UI;


namespace DarkJimmy
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField]
        private RectTransform menuContainer;
        [SerializeField]
        private Menu.Menus startingMenu;
        public int PageIndex { get; set; }


        private Menu currentMenu;
        private Menu lastMenu;
        private readonly Stack<Menu> menuStack = new Stack<Menu>();
        
        private readonly Vector2 canvasResolition =  new Vector2(2960,1440);

        private void Start()
        {
            Input.multiTouchEnabled = false;
            OpenMenu(startingMenu);
        }
        public void OpenMenu(Menu.Menus menu)
        {

            if (menuStack.Count > 0)
                lastMenu = menuStack.Peek();

            if (Menu.MenuPaths.TryGetValue(menu, out string path))
            {
                var prefab = Resources.Load<Menu>(path);

                if (prefab == null)
                    return;

                currentMenu = Instantiate(prefab, menuContainer);
                menuStack.Push(currentMenu);

                if (currentMenu.menuRank.Equals(Menu.MenuRank.Seconder))
                    lastMenu.gameObject.SetActive(false);

            }
        }
        public void OpenPopup()
        {

        }
        public void GoBack()
        {
            if (menuStack.Count.Equals(1))
                return;

            Menu item = menuStack.Pop();

            Destroy(item.gameObject);

            Menu lastMenu = menuStack.Peek();

            //if (lastMenu.menuType.Equals(Menu.Menus.Lobby) || lastMenu.menuType.Equals(Menu.Menus.Play))
            //    return;

            if(!lastMenu.gameObject.activeSelf)
                lastMenu.gameObject.SetActive(true);

        }
        public void Cancel()
        {

        }
        public void OpenUrl(string address)
        {
            if (string.IsNullOrEmpty(address))
                return;

            Application.OpenURL(address);
        }
        public int GetStackCount()
        {
            return menuStack.Count;
        }
        public Vector2 GetReferenceResolotion()
        {
            return canvasResolition;
        }
        public Menu GetCurrentMenu()
        {
            return menuStack.Peek();
        }
  
    }
}

