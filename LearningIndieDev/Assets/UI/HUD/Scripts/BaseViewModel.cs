using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Noesis;
using UnityEngine;

public class BaseViewModel : MonoBehaviour, INotifyPropertyChanged
{

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropetyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Yep, Yoda's Gay");

        }     
    }

    private Visibility _ButtonHasBeenClicked = Visibility.Collapsed;
    public Visibility ButtonHasBeenClicked 
    {
        get => _ButtonHasBeenClicked; 
        set
        {
            if (_ButtonHasBeenClicked == value)
            return;

            _ButtonHasBeenClicked = value;
            OnPropetyChanged();
        }        
    }
}
