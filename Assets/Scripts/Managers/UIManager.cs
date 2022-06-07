using System.Collections.Generic;
using UnityEngine;
using DarkJimmy.UI;
using UnityEngine.UI;


namespace DarkJimmy
{
    public class UIManager : Singleton<UIManager>
    {
        public Camera MainCamera { get; set; }
        public Menu.Menus startingMenu;
        public GameObject postProcess;

        public int PageIndex { get; set; }
        private Menu _currentMenu;
        private Stack<Menu> _stack = new Stack<Menu>();
        private readonly Vector2 canvasResolition =  new Vector2(2960,1440);
        public delegate void UpdateState(Stats state,float amount);  
        public UpdateState updateState;
        public UpdateState addCollectable;
        public override void Awake()
        {
            base.Awake();
           // Instance = this;
            LanguageManager.DefaultLanguage();
        }
        private void Start()
        {
            Input.multiTouchEnabled = false;
            Open(startingMenu);
        }
        public void Open(Menu.Menus menu)
        {
            if (_stack.Count > 1)
                _stack.Peek().gameObject.SetActive(false);

            if (Menu.MenuPaths.TryGetValue(menu, out string path))
            {
                var prefab = Resources.Load<Menu>(path);

                if (prefab == null)
                    return;

                _currentMenu = Instantiate(prefab, transform);
                _stack.Push(_currentMenu);
            }
        }
        public void GoBack()
        {
            if (_stack.Count.Equals(1))
                return;

            Menu item = _stack.Pop();

            Destroy(item.gameObject);

            Menu lastMenu = _stack.Peek();

            if (lastMenu.menuType.Equals(Menu.Menus.Lobby) || lastMenu.menuType.Equals(Menu.Menus.Play))
                return;
            lastMenu.gameObject.SetActive(true);
        }
        public int GetStackCount()
        {
            return _stack.Count;
        }
        public Menu GetCurrentMenu()
        {
            return _currentMenu;
        }
        public Vector2 GetReferenceResolotion()
        {
            return canvasResolition;
        }
  
    }
}

