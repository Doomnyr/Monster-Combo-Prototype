using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_View : MonoBehaviour
{
    public UI_ViewSO MainviewData;

    public GameObject ContainerTop;
    public GameObject ContainerCenter;
    public GameObject ContainerBottom;

    private Image _imageTop;
    private Image _imageCetner;
    private Image _imageBottom;

    private VerticalLayoutGroup _verticalLayoutGroup;

    private void Awake()
    {
        InitMainView();
    }

    public void InitMainView()
    {
        Setup();
        Configure();
    }

    public void Setup()
    {
        _verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        _imageTop = ContainerTop.GetComponent<Image>();
        _imageCetner = ContainerCenter.GetComponent<Image>();
        _imageBottom = ContainerBottom.GetComponent<Image>();
    }

    public void Configure()
    {
        _verticalLayoutGroup.padding = MainviewData.padding;
        _verticalLayoutGroup.spacing = MainviewData.spacing;
    }

    private void OnValidate()
    {
        InitMainView();
    }
}
