using System.ComponentModel;
using System.Runtime.CompilerServices;
using Noesis;
using NoesisGUIExtensions;
using UnityEngine;

public class BaseViewModel : MonoBehaviour, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropetyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public DelegateCommand YodaIsGayCommand => new DelegateCommand(YodaIsGay);

    private void YodaIsGay()
    {
        Debug.Log("Yoda is gay button has been clicked.");
        if( YodasSexualityVisibility == Visibility.Visible )
        {
            YodasSexualityVisibility = Visibility.Collapsed;                
        }
        else
        {
            YodasSexualityVisibility = Visibility.Visible;    
        }        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NoesisView noesisView = GetComponent<NoesisView>();
        if(noesisView != null)
        {
            ((FrameworkElement)noesisView.Content).DataContext = this;
        }
    }

    private Visibility _YodasSexualityVisibility = Visibility.Collapsed;
    public Visibility YodasSexualityVisibility 
    {
        get => _YodasSexualityVisibility; 
        set
        {
            if (_YodasSexualityVisibility == value)
            return;

            _YodasSexualityVisibility = value;
            OnPropetyChanged();
        }        
    }

        private double _MyProgressBarValue = 0;
        public double MyProgressBarValue    
        {
            get => _MyProgressBarValue; 
            set
            {
                if (_MyProgressBarValue == value)
                return;

                _MyProgressBarValue = value;
                OnPropetyChanged();
            }        
        }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (MyProgressBarValue < 200)
            {
                MyProgressBarValue++;
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (MyProgressBarValue > 0)
            {
                MyProgressBarValue--;
            }
        }
    }
}
