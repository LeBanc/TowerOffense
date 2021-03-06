﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AutoScroll class manages the UI navigation (keyboard) of an associated ScrollRect
/// It can be used with ScrollRect as is or with LayoutGroup elements (in its Viewport Content)
/// </summary>

// AutoScroll requires the GameObject to have a ScrollRect component
[RequireComponent(typeof(ScrollRect))]
public class AutoScroll : MonoBehaviour
{
    // Selectables to navigate to when exiting the ScrollRect
    public Selectable leftExit;
    public Selectable rightExit;
    public Selectable upExit;
    public Selectable downExit;

    // Events
    public delegate void AutoScrollEventHandler(Selectable _selecetable);
    public event AutoScrollEventHandler OnUpExitNavigationSet;
    public event AutoScrollEventHandler OnDownExitNavigationSet;
    public event AutoScrollEventHandler OnLeftExitNavigationSet;
    public event AutoScrollEventHandler OnRightExitNavigationSet;

    // private UI elements (from ScrollRect and LayoutGroup)
    ScrollRect scrollRect;
    Scrollbar verticalScrollbar;
    Scrollbar horizontalScrollbar;
    RectTransform content;
    LayoutGroupType layoutGroupType;
    GridLayoutGroup gridGroup;

    // List of all GameObjects in the ScrollRect
    List<GameObject> goList = new List<GameObject>();
    // Currently selected GameObject
    GameObject selectedObject;

    // padding value
    float scrollPaddingUp = 0f;
    float scrollPaddingDown = 0f;
    float scrollPaddingLeft = 0f;
    float scrollPaddingRight = 0f;

    public List<GameObject> List
    {
        get { return goList; }
    }

    /// <summary>
    /// At Awake, fetches the UI elements, inits the used value and the GameObject list and subscribe to events
    /// </summary>
    void Awake()
    {
        // Init AutoScroll parameters
        scrollRect = GetComponent<ScrollRect>();
        verticalScrollbar = scrollRect.verticalScrollbar;
        horizontalScrollbar = scrollRect.horizontalScrollbar;
        content = scrollRect.viewport.GetChild(0).GetComponent<RectTransform>();

        // Search for the used LayoutGroup and init its data
        if (content.TryGetComponent<HorizontalLayoutGroup>(out HorizontalLayoutGroup _hLayoutGroup))
        {
            scrollPaddingUp = _hLayoutGroup.padding.top;
            scrollPaddingDown = _hLayoutGroup.padding.bottom;
            scrollPaddingLeft = _hLayoutGroup.padding.left;
            scrollPaddingRight = _hLayoutGroup.padding.right;
            layoutGroupType = LayoutGroupType.horizontal;
        }
        else if (content.TryGetComponent<VerticalLayoutGroup>(out VerticalLayoutGroup _vLayoutGroup))
        {
            scrollPaddingUp = _vLayoutGroup.padding.top;
            scrollPaddingDown = _vLayoutGroup.padding.bottom;
            scrollPaddingLeft = _vLayoutGroup.padding.left;
            scrollPaddingRight = _vLayoutGroup.padding.right;
            layoutGroupType = LayoutGroupType.vertical;
        }
        else if (content.TryGetComponent<GridLayoutGroup>(out GridLayoutGroup _gLayoutGroup))
        {
            scrollPaddingUp = _gLayoutGroup.padding.top;
            scrollPaddingDown = _gLayoutGroup.padding.bottom;
            scrollPaddingLeft = _gLayoutGroup.padding.left;
            scrollPaddingRight = _gLayoutGroup.padding.right;
            layoutGroupType = LayoutGroupType.grid;
            gridGroup = _gLayoutGroup;
        }
        else
        {
            layoutGroupType = LayoutGroupType.none;
        }

        // Subscribe internal events
        if(upExit!=null) OnUpExitNavigationSet += SetNavFromUpExit;
        if(downExit != null) OnDownExitNavigationSet += SetNavFromDownExit;
        if(leftExit != null) OnLeftExitNavigationSet += SetNavFromLeftExit;
        if(rightExit != null) OnRightExitNavigationSet += SetNavFromRightExit;

        // Set initial navigation
        for (int i = 0; i < content.childCount; i++)
        {
            goList.Add(content.GetChild(i).gameObject);
            // If layout Horizontal or Vertical, set navigation for each added game object
            if (layoutGroupType == LayoutGroupType.horizontal || layoutGroupType == LayoutGroupType.vertical) AddNavigationHorzVert(goList[goList.Count-1]);
        }
        // If layout Grid, set Navigation for any game object in the list (for loop in SetNavigation for grid layout)
        if (layoutGroupType == LayoutGroupType.grid && goList.Count > 0) StartCoroutine(WaitAndSetNavigationGrid(goList[0]));

        // If list isn't null, set navigation from exit buttons to the first object in the list
        if (goList.Count > 0) SetNavFromExits(goList[0]);

    }

    /// <summary>
    /// OnDestroy, unsubscribe from events
    /// </summary>
    private void OnDestroy()
    {
        OnUpExitNavigationSet -= SetNavFromUpExit;
        OnDownExitNavigationSet -= SetNavFromDownExit;
        OnLeftExitNavigationSet -= SetNavFromLeftExit;
        OnRightExitNavigationSet -= SetNavFromRightExit;
    }

    /// <summary>
    /// AddPrefab method adds an instance of a prefab to the ScrollRect
    /// </summary>
    /// <param name="_prefab">Prefab to instantiate (GameObject)</param>
    public void AddPrefab(GameObject _prefab)
    {
        // Instantiate the GameObject in the content
        GameObject _goInstance = Instantiate(_prefab, content.transform);
        // Add the GameObject to the AutoScroll list and updates it
        AddGameObject(_goInstance);
    }

    /// <summary>
    /// AddPrefabReturnInstance method adds an instance of a prefab to the ScrollRect and return its
    /// </summary>
    /// <param name="_prefab">Prefab to instantiate (GameObject)</param>
    /// <returns>Created instance (GameObject)</returns>
    public GameObject AddPrefabReturnInstance(GameObject _prefab)
    {
        // Instantiate the GameObject in the content
        GameObject _goInstance = Instantiate(_prefab, content.transform);
        // Add the GameObject to the AutoScroll list and updates it
        AddGameObject(_goInstance);
        return _goInstance;
    }

    /// <summary>
    /// AddGameObject method adds a GameObject to the List (!The GameObject should already be a child of the Content!)
    /// </summary>
    /// <param name="_go">GameObject to instantiate (GameObject)</param>
    public void AddGameObject(GameObject _go)
    {
        // Add GameObject to the content list
        goList.Add(_go);
        // Set the navigation after the addition of the new GameObject
        if (layoutGroupType == LayoutGroupType.grid)
        {
            StartCoroutine(WaitAndSetNavigationGrid(_go));
        }
        else
        {
            AddNavigationHorzVert(_go);
        }
        // Set the exit navigation
        SetNavFromExits(_go);
    }

    /// <summary>
    /// SelectFirtsItem method selects the first Selectable of the AutoScroll list of GameObject
    /// </summary>
    /// <returns>GameObject of the first Selectable of the list (GameObject)</returns>
    public GameObject SelectFirtsItem()
    {
        for(int i=0; i<goList.Count;i++)
        {
            if(goList[i].TryGetComponent(out Selectable _selectable))
            {
                StartCoroutine(SelectAfterEndOfFrame(_selectable));
                return goList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// SelectAfterEndOfFrame coroutine selects the chosen Selectable after the EndOfFrame (after EventSystem update)
    /// </summary>
    /// <param name="_selectable">Chosen Selectable (Selectable)</param>
    /// <returns></returns>
    private IEnumerator SelectAfterEndOfFrame(Selectable _selectable)
    {
        yield return new WaitForEndOfFrame();
        _selectable.Select();
    }

    /// <summary>
    /// Clear method clears the AuoScroll content (destoying all GameObject in Content) and reset the exit UI navigation
    /// </summary>
    public void Clear()
    {
        foreach(GameObject go in goList)
        {
            Destroy(go);
        }
        goList.Clear();

        if (upExit != null)
        {
            OnUpExitNavigationSet?.Invoke(null);
        }
        if (downExit != null)
        {
            OnDownExitNavigationSet?.Invoke(null);
        }
        if (leftExit != null)
        {
            OnLeftExitNavigationSet?.Invoke(null);
        }
        if (rightExit != null)
        {
            OnRightExitNavigationSet?.Invoke(null);
        }
    }

    /// <summary>
    /// Remove methdo removes a specific element of the list
    /// </summary>
    /// <param name="_goInstance">Object to remove (GameObject)</param>
    public void Remove(GameObject _goInstance)
    {
        // Get the index of the GameObject
        int _index = goList.IndexOf(_goInstance);
        // If the GameObject is found in the list, index > -1
        if (_index > -1)
        {
            switch(layoutGroupType)
            {
                // If the layout type is Grid, remove the object from the list and update the UI navigation
                case LayoutGroupType.grid:
                    goList.Remove(_goInstance);
                    if (goList.Count > 0)
                    {
                        StartCoroutine(WaitAndSetNavigationGrid(goList[0]));
                        SetNavFromExits(goList[Mathf.Max(0, _index - 1)]);
                    }
                    break;
                // If there is no layout type, remove the object from the list and update the UI navigation
                case LayoutGroupType.none:
                    goList.Remove(_goInstance);
                    if ((_index - 1) >= 0 && (_index - 1) < goList.Count) StartCoroutine(WaitAndSetNavigationNone(goList[_index - 1]));
                    if (_index < goList.Count) StartCoroutine(WaitAndSetNavigationNone(goList[_index]));
                    break;

                // If the layout group is horizontal
                case LayoutGroupType.horizontal:
                    if(_index > 0) // There is an element before
                    {
                        if(goList[_index-1].TryGetComponent(out Selectable _prevSel))
                        {
                            Navigation _prevNav = _prevSel.navigation;
                            if ((_index + 1) < goList.Count) // Set the right navigation of the previous element to the next element
                            {
                                _prevNav.selectOnRight = goList[_index+ 1].TryGetComponent(out Selectable _nextSel) ? _nextSel : null;
                            }
                            else if (rightExit != null) // Set the right navigation of the previous element to the right exit and the left nav of the right exit to the previous element
                            {
                                _prevNav.selectOnRight = rightExit;
                                OnRightExitNavigationSet?.Invoke(_prevSel);
                            }
                            _prevSel.navigation = _prevNav;
                        }
                    }
                    if(_index + 1 < goList.Count) // There is an element after
                    {
                        if (goList[_index + 1].TryGetComponent(out Selectable _nextSel))
                        {
                            Navigation _nextNav = _nextSel.navigation;
                            if(_index > 0) // Set the left navigation of the next element to the previous element
                            {
                                _nextNav.selectOnLeft = goList[_index + 1].TryGetComponent(out Selectable _prevSel) ? _prevSel : null;
                            }
                            else if(leftExit != null) // Set the left navigation of the next element to the left exit and the right nav of the left exit to the next element
                            {
                                _nextNav.selectOnLeft = leftExit;
                                OnLeftExitNavigationSet?.Invoke(_nextSel);
                            }
                            _nextSel.navigation = _nextNav;
                        }
                    }
                    goList.RemoveAt(_index);
                    break;
                // If the layout group is vetical
                case LayoutGroupType.vertical:
                    if (_index > 0) // There is an element before
                    {
                        if (goList[_index - 1].TryGetComponent(out Selectable _prevSel))
                        {
                            Navigation _prevNav = _prevSel.navigation;
                            if ((_index + 1) < goList.Count) // Set the down navigation of the previous element to the next element
                            {
                                _prevNav.selectOnDown = goList[_index + 1].TryGetComponent(out Selectable _nextSel) ? _nextSel : null;
                            }
                            else if (downExit != null) // Set the down navigation of the previous element to the down exit and the up nav of the dwon exit to the previous element
                            {
                                _prevNav.selectOnDown = downExit;
                                OnDownExitNavigationSet?.Invoke(_prevSel);
                            }
                            _prevSel.navigation = _prevNav;
                        }
                    }
                    if (_index + 1 < goList.Count) // There is an element after
                    {
                        if (goList[_index + 1].TryGetComponent(out Selectable _nextSel))
                        {
                            Navigation _nextNav = _nextSel.navigation;
                            if (_index > 0) // Set the up navigation of the next element to the previous element
                            {
                                _nextNav.selectOnUp = goList[_index + 1].TryGetComponent(out Selectable _prevSel) ? _prevSel : null;
                            }
                            else if (upExit != null) // Set the up navigation of the next element to the up exit and the down nav of the up exit to the next element
                            {
                                _nextNav.selectOnUp = upExit;
                                OnUpExitNavigationSet?.Invoke(_nextSel);
                            }
                            _nextSel.navigation = _nextNav;
                        }
                    }
                    goList.RemoveAt(_index);
                    break;
            }
        }

        // Select the new element at the index if there is one
        if (_index < goList.Count) if (goList[_index].TryGetComponent(out Selectable _toSelect)) _toSelect.Select();
        // Destroy the GameObject (now that it has been removed from the list)
        Destroy(_goInstance);
    }

    /// <summary>
    /// WaitAndSetNavigationGrid coroutine waits for EndOfFrame and updates the Navigation (after EventSytem update)
    /// </summary>
    /// <param name="_goInstance">Added GameObject</param>
    /// <returns></returns>
    IEnumerator WaitAndSetNavigationGrid(GameObject _goInstance)
    {
        // Coroutine to wait for the end of frame because Grid layout need the LateUpdate to get the rows and columns counts after the addition of the new object
        yield return new WaitForEndOfFrame();
        SetNavigationGrid(_goInstance);
    }

    /// <summary>
    /// WaitAndSetNavigationNone coroutine waits for EndOfFrame and updates the Navigation (after EventSytem update)
    /// </summary>
    /// <param name="_goInstance">Added GameObject</param>
    /// <returns></returns>
    IEnumerator WaitAndSetNavigationNone(GameObject _goInstance)
    {
        yield return new WaitForEndOfFrame();
        if (_goInstance.TryGetComponent(out Selectable _sel))
        {
            Navigation _nav = _sel.navigation;
            _nav.mode = Navigation.Mode.Automatic;
            _sel.navigation = _nav;
        }
    }

    /// <summary>
    /// AddNavigationHorzVert method setups the navigation to the added GameObject (and its Selectable)
    /// </summary>
    /// <param name="_goInstance">Added GameObject</param>
    void AddNavigationHorzVert(GameObject _goInstance)
    {
        // If GO has a Selectable component
        if (_goInstance.TryGetComponent(out Selectable _sel))
        {
            Navigation _nav = new Navigation();
            int _index = goList.IndexOf(_sel.gameObject);
            if (_index < 0) return;

            // Init navigation (all selectOn objects are the exits if they exist)
            _nav.mode = Navigation.Mode.Explicit;
            if (upExit != null) _nav.selectOnUp = upExit;
            if (downExit != null) _nav.selectOnDown = downExit;
            if (leftExit != null) _nav.selectOnLeft = leftExit;
            if (rightExit != null) _nav.selectOnRight = rightExit;

            // Set explicit navigation depending on the layout group
            switch (layoutGroupType)
            {
                // If horizontal, go to previous GO on Left and come from previous GO on Right
                case LayoutGroupType.horizontal:
                    // Set left navigation of every object but the first as the previous object on the list
                    if (_index > 0)
                    {
                        if (goList[_index - 1].TryGetComponent<Selectable>(out Selectable _prevSel))
                        {
                            _nav.selectOnLeft = _prevSel;
                            Navigation _prevNav = _prevSel.navigation;
                            _prevNav.selectOnRight = _sel;
                            _prevSel.navigation = _prevNav;
                        }
                    }
                    else // set the leftExit selectOnRight as the first object of the list
                    {
                        if (leftExit != null)
                        {
                            OnLeftExitNavigationSet?.Invoke(_sel);
                        }
                    }

                    // Set right navigation of every object but the last as the next object on the list
                    if (_index < goList.Count - 1)
                    {
                        if (goList[_index + 1].TryGetComponent<Selectable>(out Selectable _nextSel))
                        {
                            _nav.selectOnRight = _nextSel;
                            Navigation _prevNav = _nextSel.navigation;
                            _prevNav.selectOnLeft = _sel;
                            _nextSel.navigation = _prevNav;
                        }
                    }
                    else // set the rightExit selectOnLeft as the last object of the list
                    {
                        if (rightExit != null)
                        {
                            OnRightExitNavigationSet?.Invoke(_sel);
                        }
                    }
                    _sel.navigation = _nav;
                    break;
                // If vertical, go to previous GO on Up and come from previous GO on Down
                case LayoutGroupType.vertical:
                    // Set up navigation of every object but the first as the previous object on the list
                    if (_index > 0)
                    {
                        if (goList[_index - 1].TryGetComponent<Selectable>(out Selectable _prevSel))
                        {
                            _nav.selectOnUp = _prevSel;
                            Navigation _prevNav = _prevSel.navigation;
                            _prevNav.selectOnDown = _sel;
                            _prevSel.navigation = _prevNav;
                        }
                    }
                    else // set the upExit selectOnDown as the first object of the list
                    {
                        if (upExit != null)
                        {
                            OnUpExitNavigationSet?.Invoke(_sel);
                        }
                    }

                    // Set down navigation of every object but the last as the next object on the list
                    if (_index < goList.Count - 1)
                    {
                        if (goList[_index + 1].TryGetComponent<Selectable>(out Selectable _nextSel))
                        {
                            _nav.selectOnDown = _nextSel;
                            Navigation _prevNav = _nextSel.navigation;
                            _prevNav.selectOnUp = _sel;
                            _nextSel.navigation = _prevNav;
                        }
                    }
                    else // set the downExit selectOnUp as the last object of the list
                    {
                        if (downExit != null)
                        {
                            OnDownExitNavigationSet?.Invoke(_sel);
                        }
                    }
                    _sel.navigation = _nav;
                    break;
                case LayoutGroupType.none:
                    _nav.mode = Navigation.Mode.Automatic;
                    break;
            }
        }
    }

    /// <summary>
    /// SetNavigationGrid method updates the UI Navigation of all GameObject of the list after a new one was added
    /// </summary>
    /// <param name="_goInstance">Added GameObject</param>
    void SetNavigationGrid(GameObject _goInstance)
    {
        // If GO has a Selectable component
        if (_goInstance.TryGetComponent(out Selectable _sel))
        {
            Navigation _nav = new Navigation();
            int _index = goList.IndexOf(_sel.gameObject);
            if (_index < 0) return;

            // Init navigation (all selectOn objects are the exits if they exist)
            _nav.mode = Navigation.Mode.Explicit;
            if (upExit != null) _nav.selectOnUp = upExit;
            if (downExit != null) _nav.selectOnDown = downExit;
            if (leftExit != null) _nav.selectOnLeft = leftExit;
            if (rightExit != null) _nav.selectOnRight = rightExit;

            // Get rows and colums constraints
            int _columnsCount = Mathf.FloorToInt((content.rect.width - gridGroup.padding.left - gridGroup.padding.right + gridGroup.spacing.x) / (gridGroup.cellSize.x + gridGroup.spacing.x));
            int _rowsCount = Mathf.FloorToInt((content.rect.height - gridGroup.padding.top - gridGroup.padding.bottom + gridGroup.spacing.y) / (gridGroup.cellSize.y + gridGroup.spacing.y));

            if (gridGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount) _rowsCount = gridGroup.constraintCount;
            if (gridGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount) _columnsCount = gridGroup.constraintCount;

            // Set navigation for each item in the GameObject list
            for (int _cellIndex = 0; _cellIndex < goList.Count; _cellIndex++)
            {
                if (goList[_cellIndex].TryGetComponent<Selectable>(out Selectable _selectable))
                {
                    _nav = new Navigation();
                    _nav.mode = Navigation.Mode.Explicit;
                    if (upExit != null) _nav.selectOnUp = upExit;
                    if (downExit != null) _nav.selectOnDown = downExit;
                    if (leftExit != null) _nav.selectOnLeft = leftExit;
                    if (rightExit != null) _nav.selectOnRight = rightExit;

                    // Horizontal filling
                    if (gridGroup.startAxis == GridLayoutGroup.Axis.Horizontal)
                    {
                        // Fill from left
                        if (gridGroup.startCorner == GridLayoutGroup.Corner.UpperLeft || gridGroup.startCorner == GridLayoutGroup.Corner.LowerLeft)
                        {
                            // Left selectable
                            if ((_cellIndex % _columnsCount) > 0) // If not first column
                            {
                                if (goList[_cellIndex - 1].TryGetComponent(out Selectable _leftSelectable)) // Set the element at left as the left selectable
                                {
                                    _nav.selectOnLeft = _leftSelectable;
                                }
                            }
                            // Right selectable
                            if ((_cellIndex % _columnsCount) < (_columnsCount - 1)) // If not on last column
                            {
                                if ((_cellIndex + 1) < goList.Count) // If there is an element on the right, set it as the right selectable
                                {
                                    if (goList[_cellIndex + 1].TryGetComponent(out Selectable _rightSelectable))
                                    {
                                        _nav.selectOnRight = _rightSelectable;
                                    }
                                }
                                else if ((_cellIndex + 1 - _columnsCount) > 0) // else if there is an element at the right of the element above, set it as right selectable
                                {
                                    if (goList[_cellIndex + 1 - _columnsCount].TryGetComponent(out Selectable _rightSelectable))
                                    {
                                        _nav.selectOnRight = _rightSelectable;
                                    }
                                }
                            }
                        }
                        else // Fill from right
                        {
                            // Right selectable
                            if ((_cellIndex % _columnsCount) > 0) // If not first column
                            {
                                if (goList[_cellIndex - 1].TryGetComponent(out Selectable _rightSelectable)) // Set the element at right as the right selectable
                                {
                                    _nav.selectOnRight = _rightSelectable;
                                }
                            }
                            // Left selectable
                            if ((_cellIndex % _columnsCount) < (_columnsCount - 1)) // If not on last column
                            {
                                if ((_cellIndex + 1) < goList.Count) // If there is an element on the left, set it as the left selectable
                                {
                                    if (goList[_cellIndex + 1].TryGetComponent(out Selectable _leftSelectable))
                                    {
                                        _nav.selectOnLeft = _leftSelectable;
                                    }
                                }
                            }
                            else if ((_cellIndex + 1 - _columnsCount) > 0) // else if there is an element at the left of the element above, set it as left selectable
                            {
                                if (goList[_cellIndex + 1 - _columnsCount].TryGetComponent(out Selectable _leftSelectable))
                                {
                                    _nav.selectOnLeft = _leftSelectable;
                                }
                            }
                        }

                        // Fill from top
                        if (gridGroup.startCorner == GridLayoutGroup.Corner.UpperLeft || gridGroup.startCorner == GridLayoutGroup.Corner.UpperRight)
                        {
                            // Up selectable
                            if (_cellIndex >= _columnsCount) // If not first row, set the element above as the up selectable
                            {
                                if (goList[_cellIndex - _columnsCount].TryGetComponent(out Selectable _upSelectable))
                                {
                                    _nav.selectOnUp = _upSelectable;
                                }
                            }
                            // Down selectable
                            if (_cellIndex < (_columnsCount * (_rowsCount - 1))) // If not last row
                            {
                                if ((_cellIndex + _columnsCount) < goList.Count) // If there is an element below the selected one, set it as down selectable
                                {
                                    if (goList[_cellIndex + _columnsCount].TryGetComponent(out Selectable _downSelectable))
                                    {
                                        _nav.selectOnDown = _downSelectable;
                                    }
                                }
                                else // Else set the last element of the list as down selectable
                                {
                                    if (goList[goList.Count - 1].TryGetComponent(out Selectable _downSelectable))
                                    {
                                        _nav.selectOnDown = _downSelectable;
                                    }
                                }
                            }
                        }
                        else // Fill from bottom
                        {
                            // Down selectable
                            if (_cellIndex >= _columnsCount) // If not first row, set the element below as the down selectable
                            {
                                if (goList[_cellIndex - _columnsCount].TryGetComponent(out Selectable _downSelectable))
                                {
                                    _nav.selectOnDown = _downSelectable;
                                }
                            }
                            // Up selectable
                            if (_cellIndex < (_columnsCount * (_rowsCount - 1))) // If not last row
                            {
                                if ((_cellIndex + _columnsCount) < goList.Count) // If there is an element above the selected one, set it as up selectable
                                {
                                    if (goList[_cellIndex + _columnsCount].TryGetComponent(out Selectable _upSelectable))
                                    {
                                        _nav.selectOnUp = _upSelectable;
                                    }
                                }
                                else // Else set the last element of the list as up selectable
                                {
                                    if (goList[goList.Count - 1].TryGetComponent(out Selectable _upSelectable))
                                    {
                                        _nav.selectOnUp = _upSelectable;
                                    }
                                }
                            }
                        }
                    }
                    else // Vertical filling
                    {
                        // Fill from top
                        if (gridGroup.startCorner == GridLayoutGroup.Corner.UpperLeft || gridGroup.startCorner == GridLayoutGroup.Corner.UpperRight)
                        {
                            if ((_cellIndex % _rowsCount) > 0) // If not first row
                            {
                                if (goList[_cellIndex - 1].TryGetComponent(out Selectable _upSelectable)) // Element above is up selectable
                                {
                                    _nav.selectOnUp = _upSelectable;
                                }
                            }

                            if ((_cellIndex % _rowsCount) < (_rowsCount - 1)) // If not on last row
                            {
                                if ((_cellIndex + 1) < goList.Count) // If there is an element below, set it as down selectable
                                {
                                    if (goList[_cellIndex + 1].TryGetComponent(out Selectable _downSelectable))
                                    {
                                        _nav.selectOnDown = _downSelectable;
                                    }
                                }
                                else // Set the next element of the previous column
                                {
                                    if (goList[_cellIndex - _rowsCount + 1].TryGetComponent(out Selectable _downSelectable))
                                    {
                                        _nav.selectOnDown = _downSelectable;
                                    }
                                }
                            }
                        }
                        else // fill from bottom
                        {
                            if ((_cellIndex % _rowsCount) > 0) // If not first row
                            {
                                if (goList[_cellIndex - 1].TryGetComponent(out Selectable _downSelectable)) // Element below is down selectable
                                {
                                    _nav.selectOnDown = _downSelectable;
                                }
                            }

                            if ((_cellIndex % _rowsCount) < (_rowsCount - 1)) // If not on last row
                            {
                                if ((_cellIndex + 1) < goList.Count) // If there is an element above, set it as up selectable
                                {
                                    if (goList[_cellIndex + 1].TryGetComponent(out Selectable _upSelectable))
                                    {
                                        _nav.selectOnUp = _upSelectable;
                                    }
                                }
                                else // Set the next element of the previous column
                                {
                                    if (goList[_cellIndex - _rowsCount + 1].TryGetComponent(out Selectable _upSelectable))
                                    {
                                        _nav.selectOnUp = _upSelectable;
                                    }
                                }
                            }
                        }

                        // Fill from left
                        if (gridGroup.startCorner == GridLayoutGroup.Corner.UpperLeft || gridGroup.startCorner == GridLayoutGroup.Corner.LowerLeft)
                        {
                            // Left selectable
                            if (_cellIndex >= _rowsCount) // If not first column, set the element left as the left selectable
                            {
                                if (goList[_cellIndex - _rowsCount].TryGetComponent(out Selectable _leftSelectable))
                                {
                                    _nav.selectOnLeft = _leftSelectable;
                                }
                            }
                            // Right selectable
                            if (_cellIndex < (_rowsCount * (_columnsCount - 1))) // If not last column
                            {
                                if ((_cellIndex + _rowsCount) < goList.Count) // If there is an element at the right of the selected one, set it as right selectable
                                {
                                    if (goList[_cellIndex + _rowsCount].TryGetComponent(out Selectable _rightSelectable))
                                    {
                                        _nav.selectOnRight = _rightSelectable;
                                    }
                                }
                                else // Else set the last element of the list as right selectable
                                {
                                    if (goList[goList.Count - 1].TryGetComponent(out Selectable _rightSelectable))
                                    {
                                        _nav.selectOnRight = _rightSelectable;
                                    }
                                }
                            }
                        }
                        else // fill from right
                        {
                            // Right selectable
                            if (_cellIndex >= _rowsCount) // If not first column, set the element right as the right selectable
                            {
                                if (goList[_cellIndex - _rowsCount].TryGetComponent(out Selectable _rightSelectable))
                                {
                                    _nav.selectOnRight = _rightSelectable;
                                }
                            }
                            // Left selectable
                            if (_cellIndex < (_rowsCount * (_columnsCount - 1))) // If not last column
                            {
                                if ((_cellIndex + _rowsCount) < goList.Count) // If there is an element at the left of the selected one, set it as left selectable
                                {
                                    if (goList[_cellIndex + _rowsCount].TryGetComponent(out Selectable _leftSelectable))
                                    {
                                        _nav.selectOnLeft = _leftSelectable;
                                    }
                                }
                                else // Else set the last element of the list as left selectable
                                {
                                    if (goList[goList.Count - 1].TryGetComponent(out Selectable _leftSelectable))
                                    {
                                        _nav.selectOnLeft = _leftSelectable;
                                    }
                                }
                            }
                        }
                    }
                    _selectable.navigation = _nav;
                }
            }
        }
    }

    /// <summary>
    /// At Update, check if the currently selected object (by EventSystem) is part of the AutoScroll and call AutoScrollOnObject method if it is
    /// </summary>
    private void Update()
    {
        GameObject _go = EventSystem.current.currentSelectedGameObject;
        if (goList.Contains(_go))
        {
            if (selectedObject != _go)
            {
                selectedObject = _go;
                AutoScrollOnObject(_go);
            }
        }
    }

    /// <summary>
    /// AutoScrollOnObject method get the position of the selected GameObject and changes the scrollBar values to display it in the ScrollRect
    /// </summary>
    /// <param name="_go">Selected GameObject (GameObject)</param>
    private void AutoScrollOnObject(GameObject _go)
    {
        // Get Viewport content positions
        RectTransform _rect = scrollRect.GetComponent<RectTransform>();
        float scrollXMin = _rect.position.x + _rect.rect.xMin * _rect.lossyScale.x;
        float scrollYMin = _rect.position.y + _rect.rect.yMin * _rect.lossyScale.y;
        float scrollXMax = _rect.position.x + _rect.rect.xMax * _rect.lossyScale.x;
        float scrollYMax = _rect.position.y + _rect.rect.yMax * _rect.lossyScale.y;

        // Get GO delta positions from content positions
        _rect = _go.GetComponent<RectTransform>();
        float deltaXMin = (_rect.position.x + _rect.rect.xMin * _rect.lossyScale.x) - scrollXMin;
        float deltaYMin = (_rect.position.y + _rect.rect.yMin * _rect.lossyScale.y) - scrollYMin;
        float deltaXMax = scrollXMax - (_rect.position.x + _rect.rect.xMax * _rect.lossyScale.x);
        float deltaYMax = scrollYMax - (_rect.position.y + _rect.rect.yMax * _rect.lossyScale.y);

        // Get ScrollBars sizes
        float hScrollSize = (horizontalScrollbar != null) ? horizontalScrollbar.gameObject.activeSelf ? horizontalScrollbar.GetComponent<RectTransform>().sizeDelta.y + scrollRect.horizontalScrollbarSpacing : 0f :0f;
        float vScrollSize = (verticalScrollbar != null) ? verticalScrollbar.gameObject.activeSelf ? verticalScrollbar.GetComponent<RectTransform>().sizeDelta.x + scrollRect.verticalScrollbarSpacing : 0f : 0f;

        // Move if the GO is not visible on the Viewport Content (any delta is negative)
        // Horizontal movement
        if(layoutGroupType == LayoutGroupType.horizontal || layoutGroupType == LayoutGroupType.grid)
        {
            if (deltaXMin < scrollPaddingLeft)
            {
                float _movement = (deltaXMin - scrollPaddingLeft) / content.lossyScale.x;
                content.anchoredPosition = new Vector2(content.anchoredPosition.x - _movement, content.anchoredPosition.y);
            }
            else if (deltaXMax < scrollPaddingRight + vScrollSize) // scrollbar is at the right of the scrollview
            {
                float _movement = (deltaXMax - scrollPaddingRight - vScrollSize) / content.lossyScale.x;
                content.anchoredPosition = new Vector2(content.anchoredPosition.x + _movement, content.anchoredPosition.y);
            }
        }

        // Vertical movement
        if (layoutGroupType == LayoutGroupType.vertical || layoutGroupType == LayoutGroupType.grid)
        {
            if (deltaYMin < scrollPaddingUp + hScrollSize) // scrollbar is at the bottom of the scrollview
            {
                float _movement = (deltaYMin - scrollPaddingDown - hScrollSize) / content.lossyScale.y;
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, content.anchoredPosition.y - _movement);
            }
            else if (deltaYMax < scrollPaddingDown)
            {
                float _movement = (deltaYMax - scrollPaddingUp) / content.lossyScale.y;
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, content.anchoredPosition.y + _movement);
            }
        }        

        SetNavFromExits(_go);
    }

    /// <summary>
    /// SetNavFromExits method changes the UI navigation of all exits to go back to the selected GameObject
    /// </summary>
    /// <param name="_go">Selected GameObject (GameObject)</param>
    private void SetNavFromExits(GameObject _go)
    {
        // Set return from exit Selectables to the current go Selectable if it exist
        if (_go.TryGetComponent<Selectable>(out Selectable _sel))
        {
            if (upExit != null)
            {
                OnUpExitNavigationSet?.Invoke(_sel);
            }
            if (downExit != null)
            {
                OnDownExitNavigationSet?.Invoke(_sel);
            }
            if (leftExit != null)
            {
                OnLeftExitNavigationSet?.Invoke(_sel);
            }
            if (rightExit != null)
            {
                OnRightExitNavigationSet?.Invoke(_sel);
            }
        }
    }

    /// <summary>
    /// SetNavFromUpExit method set the UI navigation of the UpExit
    /// </summary>
    /// <param name="_selectable">Chosen Selectable (Selectable)</param>
    private void SetNavFromUpExit(Selectable _selectable)
    {
        if(upExit != null)
        {
            Navigation _nav = upExit.navigation;
            _nav.selectOnDown = _selectable;
            upExit.navigation = _nav;
        }
    }

    /// <summary>
    /// SetNavFromDownExit method set the UI navigation of the DownExit
    /// </summary>
    /// <param name="_selectable">Chosen Selectable (Selectable)</param>
    private void SetNavFromDownExit(Selectable _selectable)
    {
        if (downExit != null)
        {
            Navigation _nav = downExit.navigation;
            _nav.selectOnUp = _selectable;
            downExit.navigation = _nav;
        }
    }

    /// <summary>
    /// SetNavFromLeftExit method set the UI navigation of the LeftExit
    /// </summary>
    /// <param name="_selectable">Chosen Selectable (Selectable)</param>
    private void SetNavFromLeftExit(Selectable _selectable)
    {
        if (leftExit != null)
        {
            Navigation _nav = leftExit.navigation;
            _nav.selectOnRight = _selectable;
            leftExit.navigation = _nav;
        }
    }

    /// <summary>
    /// SetNavFromRightExit method set the UI navigation of the RightExit
    /// </summary>
    /// <param name="_selectable">Chosen Selectable (Selectable)</param>
    private void SetNavFromRightExit(Selectable _selectable)
    {
        if (rightExit != null)
        {
            Navigation _nav = rightExit.navigation;
            _nav.selectOnLeft = _selectable;
            rightExit.navigation = _nav;
        }
    }

    /// <summary>
    /// SetNavToFirstObject method, set the exits UI navigation to the first GameObject of the list
    /// </summary>
    public void SetNavToFirstObject()
    {
        if(goList.Count > 0) SetNavFromExits(goList[0]);
    }

    /// <summary>
    /// LayoutGroupType enum used to have specific UI navigation
    /// </summary>
    enum LayoutGroupType
    {
        horizontal,
        vertical,
        grid,
        none
    }
}