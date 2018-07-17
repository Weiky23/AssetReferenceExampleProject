using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetsNamespace;

public class ClientCodeExample : MonoBehaviour
{
    public AssetReference prefab;
    public AssetReference sampleConfig;
    public AssetReference sampleAudio;

    void Start()
    { 
        // получаем из ссылки готовый инстанцированный GameObject
        GameObject instantiatedDeactivated = prefab.GetAsset();
        if (instantiatedDeactivated != null)
        {
            instantiatedDeactivated.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            instantiatedDeactivated.SetActive(true);
        }

        // получаем из ссылки кастомный ScriptableObject и выводим информацию о нем
        AssetLabels config = sampleConfig.GetObject<AssetLabels>();
        if (config != null)
        {
            Debug.Log("Asset Labels:");
            for (int i = 0; i < config.assetLabels.Length; i++)
            {
                Debug.Log(config.assetLabels[i]);
            }
        }

        // получаем из ссылки звук и проигрываем его
        AudioClip clip = sampleAudio.GetObject<AudioClip>();
        if (clip != null)
        {
            AudioSource clipPlayer = new GameObject().AddComponent<AudioSource>();
            clipPlayer.PlayOneShot(clip);
        }
    }
}
