using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SoundManager : MonoBehaviour
{
	[SerializeField] private Slider volumeSlider = null;

	private static SoundManager instance = null;

	float r = 31;  // red component
	float g = 182;  // green component
	float b = 122;  // blue component

	Color color;


	public static SoundManager Instance
	{
		get { return instance; }
	}
	void Awake()
	{
		DontDestroyOnLoad(this);

		if (instance == null)
        {
			instance = this;
        }
        else
        {
			Destroy(gameObject);
        }
	}
	


	//colora di verde la barra del livello del volume
	private void Update()
    {
		var fill = (volumeSlider as UnityEngine.UI.Slider).GetComponentsInChildren<UnityEngine.UI.Image>()
					.FirstOrDefault(t => t.name == "Fill");
		if (fill != null)
		{
			fill.color = Color.Lerp(Color.white, Color.green, (float)0.5);
		}
	}
    private void Start()
	{
		color = new Color(r, g, b);
		volumeSlider = GameObject.Find("volumeSlider").GetComponent<Slider>();
		volumeSlider.gameObject.SetActive(false);
		LoadValues();
	}

	public void SaveVolume()
	{
		float volumeValue = volumeSlider.value;
		PlayerPrefs.SetFloat("VolumeValue",volumeValue);
		LoadValues();
	}
	
	void LoadValues()
	{
		float volumeValue = PlayerPrefs.GetFloat("VolumeValue");
		volumeSlider.value = volumeValue;
		AudioListener.volume = volumeValue;
	}

	public void OpenSlider()
    {
		volumeSlider.gameObject.SetActive(true);
    }
}