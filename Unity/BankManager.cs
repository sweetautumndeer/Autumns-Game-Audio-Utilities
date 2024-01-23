using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;

public class BankManager : MonoBehaviour
{
    // Invokes callback once FMOD banks have finished loading into memory.
    // Use during loading screens, avoids FMOD Events being called before initialization (especially in single-threaded environments)
    public IEnumerator CallbackOnAllBanksLoaded(UnityAction callback)
    {
        while (!RuntimeManager.HaveAllBanksLoaded)
        {
            yield return null;
        }

        callback.Invoke();
        yield return null;
    }

    public IEnumerator CallbackOnBanksLoaded(string[] bankNames, UnityAction callback)
    {
        bool banksLoaded = false;

        while (!banksLoaded)
        {
            bool var = true;
            foreach (string bank in bankNames)
            {
                var &= RuntimeManager.HasBankLoaded(bank);
            }
            banksLoaded = var;
            yield return null;
        }

        callback.Invoke();
        yield return null;
    }

    public IEnumerator CallbackOnBanksLoaded(string bankName, UnityAction callback)
    {
        while (!RuntimeManager.HasBankLoaded(bankName))
        {
            yield return null;
        }

        callback.Invoke();
        yield return null;
    }

    public void LoadBank(string bankName)
    {
        FMODUnity.RuntimeManager.LoadBank(bankName);
        StartCoroutine(CallbackOnBanksLoaded(bankName, Test));
    }

    public void Test()
    {
        Debug.Log("Bank loaded hehe");
    }

    public void UnloadBank(string bankName)
    {
        FMODUnity.RuntimeManager.UnloadBank(bankName);
    }

    public void UnloadBank(Bank bank)
    {
        UnloadBank(GetBankName(bank));
    }

    public void UnloadAllBanks()
    {
        Bank[] banks = GetLoadedBanks();
        foreach (Bank bank in banks)
        {
            UnloadBank(bank);
        }
    }

    public string GetBankName(Bank bank)
    {
        bank.getPath(out string path);
        string[] paths = path.Split('/');
        return paths[path.Length - 1];
    }

    public int GetLoadedBankCount()
    {
        FMODUnity.RuntimeManager.StudioSystem.getBankCount(out int result);
        return result;
    }

    public Bank[] GetLoadedBanks()
    {
        // Does this return a list of ALL banks or only loaded ones?
        int count = GetLoadedBankCount();
        Bank[] banks = new Bank[count];
        FMODUnity.RuntimeManager.StudioSystem.getBankList(out banks);
        return banks;
    }
}
