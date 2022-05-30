using System.Collections.Generic;
using UnityEngine;
using DarkJimmy.UI;

namespace DarkJimmy
{
    public class UIManager : Singleton<UIManager>
    {
        public Menu.Menus startingMenu;
        public const float DELAY = 3.0f;
        public GameObject postProcess;

        private Menu _currentMenu;

        private Stack<Menu> _stack = new Stack<Menu>();
        void Start()
        {
            Open(startingMenu);
        }
        public void Open(Menu.Menus menu)
        {
            if (_stack.Count > 1)
                _stack.Peek().gameObject.SetActive(false);

            if (Menu.MenuPaths.TryGetValue(menu, out string path))
            {
                var prefab = Resources.Load<Menu>(path);
                _currentMenu = Instantiate(prefab, transform);
                _stack.Push(_currentMenu);
            }

            if (_stack.Count.Equals(2))
                postProcess.SetActive(true);
        }
        public void GoBack()
        {
            if (_stack.Count.Equals(1))
                return;

            var item = _stack.Pop();

            Destroy(item.gameObject);

            if (_stack.Count.Equals(1))
                postProcess.SetActive(false);

            var lastMenu = _stack.Peek();

            if (lastMenu.menuType.Equals(Menu.Menus.MainMenu) || lastMenu.menuType.Equals(Menu.Menus.Game))
                return;
            lastMenu.gameObject.SetActive(true);
        }

        public Menu GetCurrentMenu()
        {
            return _currentMenu;
        }

    }
}

