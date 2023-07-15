using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ResultHandler : MonoBehaviour
{
    //elementos do formulário
    [SerializeField] TMP_InputField patrimonio;
    [SerializeField] TMP_InputField IMEI;
    [SerializeField] ScreenNavigator screenNavigator;
    [SerializeField] SerializableTMPDropdown status;
    [SerializeField] SerializableTMPDropdown paradeiro;
    [SerializeField] Button salvar;
    [SerializeField] Confirmar confirmar;

    //elementos de UI
    [SerializeField] Alert alert;
    [SerializeField] LoadingScreen loadingScreen;
    // banco de dados
    [SerializeField] BdOps databaseHandler;

    // dados recuperados
    Dictionary<string, object> data = new();
    private void OnEnable()
    {
        DisableEdit();
        if (databaseHandler.workingTabletSnapShot == null) return;
        data = databaseHandler.workingTabletSnapShot.ToDictionary();
        if(data.TryGetValue("patrimonio", out object obj))
        patrimonio.text = obj.ToString();
        else  patrimonio.text = "error";

        if(data.TryGetValue("IMEI", out obj)) 
        IMEI.text = obj.ToString();
        else  IMEI.text = "error";

        if(data.TryGetValue("status", out object obj2))
        status.Dropdown.value = Convert.ToInt32(obj2);
        else status.Dropdown.value = -1;
        

        if(data.TryGetValue("paradeiro", out obj2))
        paradeiro.Dropdown.value = Convert.ToInt32(obj2);
        else paradeiro.Dropdown.value = -1;
    }

    public void EnableEdit()
    {
        patrimonio.interactable = true;
        status.Dropdown.interactable = true;
        paradeiro.Dropdown.interactable = true;
        salvar.interactable = true;
    }

    public void DisableEdit()
    {
        patrimonio.interactable = false;
        status.Dropdown.interactable = false;
        paradeiro.Dropdown.interactable = false;
        salvar.interactable= false;
    }

    public async void SobreEscrever()
    {
         TabletbaseData tabletNewData = new()
        {
            IMEI = IMEI.text,
            patrimonio = patrimonio.text,
            status = status.Dropdown.value,
            paradeiro = paradeiro.Dropdown.value
        };
        loadingScreen.OpenLoadScreen();
        await databaseHandler.OverwriteDevice(databaseHandler.workingTabletSnapShot.Id, tabletNewData);
        loadingScreen.CloseLoadScreen();
        DisableEdit();
    }

    public async void DeleteTablet()
    {
        loadingScreen.OpenLoadScreen();
        await databaseHandler.DeleteDevice(databaseHandler.workingTabletSnapShot.Id);
        loadingScreen.CloseLoadScreen();
        alert.Open("Tablet excluido!");
        DisableEdit();
    }
    public void ButtonOverwrite()
    {
        confirmar.gameObject.SetActive(true);
        confirmar.CallConfirmar(SobreEscrever);
    }
    public void ButtonDelete() 
    {
        confirmar.gameObject.SetActive(true);
        confirmar.CallConfirmar(DeleteTablet);
        confirmar.onConfirm +=()=> screenNavigator.NavigateBack();
    }

}

