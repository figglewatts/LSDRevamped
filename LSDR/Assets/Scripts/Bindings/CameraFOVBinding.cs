using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Torii.Binding;
using UnityEngine;

public class CameraFOVBinding : MonoBehaviour, IPropertyWatcher
{
    public Camera Camera;

    private BindBroker _broker;

    public float FOV
    {
        get { return Camera.fieldOfView; }
        set
        {
            Camera.fieldOfView = value;
            NotifyPropertyChange(nameof(FOV));
        }
    }

	// Use this for initialization
	void Start () {
		_broker = new BindBroker();
        _broker.RegisterData(GameSettings.CurrentSettings);
        GUID = Guid.NewGuid();

        FOV = GameSettings.CurrentSettings.FOV;

        _broker.Bind(() => GameSettings.CurrentSettings.FOV, () => FOV, BindingType.OneWay);
    }

    public event Action<string, IPropertyWatcher> OnPropertyChange;
    public Guid GUID { get; private set; }
    public void NotifyPropertyChange(string propertyName)
    {
        OnPropertyChange?.Invoke(propertyName, this);
    }
}
