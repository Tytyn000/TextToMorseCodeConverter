using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ReadInputField : MonoBehaviour
{
    public string InputText;
    public int WordsCount;
    public List<string> OutputFieldWordsList = new List<string>();
    public List<string> MorseCodeListToRead = new List<string>();
    public string[] RawListOfWordsFromInputField;
    public string[] ArrayOfMorseCodeCharacters = { ".-", "-...", "-.-.", "-..", ".", "..-.", "--.", "....", "..", ".---", "-.-", ".-..", "--", "-.", "---", ".--.", "--.-", ".-.", "...", "-", "..-", "...-", ".--", "-..-", "-.--", "--..", "-----", ".----", "..---", "...--", "....-", ".....", "-....", "--...", "---..", "----.", ".-.-.-", "--..--", "..--..", ".----.", "-.-.--", "-..-.", "-.--.", "-.--.-", ".-...", "---...", "-.-.-.", "-...-", ".-.-.", "-....-", "..--.-", "...-..-", ".--.-." };
    public string[] ArrayOfAlphabetCharacters = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", ",", "?", "'", "!", "/", "(", ")", "&", ":", ";", "=", "+", "-", "_", "$", "@" };
    public int WordsListLength;

    public int NumberOfCharactersInTheWord;
    public string CurrentWord;
    public string CurrentLetter;
    public string StringAlphabetArray;
    public string[] ArrayAlphabetArray;

    public InputField InputField;
    public InputField OutputField;

    public AudioSource AudioSource;

    public float BeepFrequency = 800f;
    public float ShortBeepDuration = 0.2f;
    public float LongBeepDuration = 0.6f;

    public bool IsUsingFirstBeepSystem = true;
    public string Difficulty;

    public bool IsReadingMorseCode;
    public int NumberOfWPM;

    public Dropdown ChooseDifficultyDropdown;
    public InputField BeepFrequencyInputField;

    public string SpacingCharacter = "space";
    public Dropdown SpacingCharacterDrowdown;

    public GameObject BeepImage;
    public bool IsImageBeepPossible;

    public Dropdown InputFieldDropdown;
    public Dropdown OutputFieldDropdown;

    public Button ReadOutputFieldButton;

    void Start()
    {
        IsReadingMorseCode = false;
        Difficulty = "Beginner";
        ShortBeepDuration = 0.24f;
        LongBeepDuration = 0.72f;
        NumberOfWPM = 5;
        BeepFrequencyInputField.text = BeepFrequency.ToString();
        if (SpacingCharacter == "/") 
        { 
            SpacingCharacterDrowdown.value = 0; 
        } 
        else 
        { 
            SpacingCharacterDrowdown.value = 1; 
        }
        ReadOutputFieldButton.interactable = InputFieldDropdown.value == 0;
        IsImageBeepPossible = true;
    }
    public void readInputField(string inputText)
    {
        if (InputFieldDropdown.value == 0)
        {
            InputText = inputText;
            RawListOfWordsFromInputField = InputText.Split(new char[] { ' ', '\n', '\r', '\t', '\a' }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < RawListOfWordsFromInputField.Length; i++)
            {
                RawListOfWordsFromInputField[i] = RawListOfWordsFromInputField[i].ToLower();
            }
            WordsListLength = RawListOfWordsFromInputField.Length;
        }
    }

    public void ConvertTextToMorseCode()
    {
        OutputFieldWordsList.Clear();
        MorseCodeListToRead.Clear();
        OutputField.text = null;
        for (int i = 0; i < WordsListLength; i++)
        {
            CurrentWord = RawListOfWordsFromInputField[i].ToString();//Sélectionne le mot
            NumberOfCharactersInTheWord = CurrentWord.Length;//Taille du mot
            for (int j = 0; j < NumberOfCharactersInTheWord; j++)//boucle qui traite chaque caractère du mot
            {
                CurrentLetter = CurrentWord[j].ToString();
                for (int k = 0; k < ArrayOfAlphabetCharacters.Length; k++)
                {
                    StringAlphabetArray = ArrayOfAlphabetCharacters[k].ToString();
                    if (CurrentLetter == StringAlphabetArray)//vérifie si le caractère choisi et le caractère[k] de l'array sont les mêmes
                    {
                        ArrayAlphabetArray[0] = new string(StringAlphabetArray.ToCharArray());
                        OutputFieldWordsList.Add(ArrayOfMorseCodeCharacters[k]);
                        MorseCodeListToRead.Add(ArrayOfMorseCodeCharacters[k]);
                        ArrayAlphabetArray[0] = null;
                    }
                }
                if (i != WordsListLength)
                {
                    OutputFieldWordsList.Add(SpacingCharacter);

                    MorseCodeListToRead.Add(" ");//3 temps de pause
                    MorseCodeListToRead.Add(" ");
                    MorseCodeListToRead.Add(" ");
                }
            }
            if (i != WordsListLength - 1)
            {
                OutputFieldWordsList.Add(SpacingCharacter);
                OutputFieldWordsList.Add(SpacingCharacter);

                MorseCodeListToRead.Add("");//7 temps de pause
                MorseCodeListToRead.Add(" ");
                MorseCodeListToRead.Add(" ");
                MorseCodeListToRead.Add(" ");
            }
        }
        for (int i = 0; i < OutputFieldWordsList.Count; i++)
        {
            OutputField.text += OutputFieldWordsList[i];
        }
    }

    public void ConvertMorseCodeToAudio()
    {
        if (IsReadingMorseCode == false)
        {
            StartCoroutine(ReadingMorseCode());
        }
    }
    public IEnumerator ReadingMorseCode()
    {
        IsReadingMorseCode = true;
        for (int i = 0; i < MorseCodeListToRead.Count ; i++)
        {
            for (int j = 0; j < MorseCodeListToRead[i].Length; j++)
            {
                print(MorseCodeListToRead[i][j]);
                string CurrentCharacter = MorseCodeListToRead[i][j].ToString();
                if (CurrentCharacter == ".")
                {
                    PlayMorseBeep(true);
                    if (IsImageBeepPossible) { BeepImage.SetActive(true); }
                    yield return new WaitForSeconds(ShortBeepDuration);//Temps de pause que le son se joue
                    if (IsImageBeepPossible) { BeepImage.SetActive(false); }
                    yield return new WaitForSeconds(ShortBeepDuration);//1 unité de pause
                }
                else if (CurrentCharacter == "-")
                {
                    PlayMorseBeep(false);
                    if (IsImageBeepPossible) { BeepImage.SetActive(true); }
                    yield return new WaitForSeconds(LongBeepDuration);//Temps que le son se joue
                    if (IsImageBeepPossible) { BeepImage.SetActive(true); }
                    yield return new WaitForSeconds(ShortBeepDuration);//1 unité de pause
                }
                else if (CurrentCharacter == " ")
                {
                    yield return new WaitForSeconds(ShortBeepDuration);//1 unité de temps 
                }
                else if (CurrentCharacter == "/")
                {
                    yield return new WaitForSeconds(ShortBeepDuration);//1 unité de temps 
                }
            }
        }
        IsReadingMorseCode = false;
        BeepImage.SetActive(true);
    }
    public void StopReadingMorseCode()
    {
        StopAllCoroutines();
        IsReadingMorseCode = false;
        BeepImage.SetActive(true);
    }
    public void ChooseLevel(int level)
    {
        switch (level)
        {
            case 0: Difficulty = "Beginner"; ShortBeepDuration = 0.24f; LongBeepDuration = 0.72f; NumberOfWPM = 5; break;
            case 1: Difficulty = "Intermediate"; ShortBeepDuration = 0.10f; LongBeepDuration = 0.30f; NumberOfWPM = 12; break;
            case 2: Difficulty = "Military"; ShortBeepDuration = 0.06f; LongBeepDuration = 0.18f; NumberOfWPM = 20; break;  
            case 3: Difficulty = "Professional"; ShortBeepDuration = 0.048f; LongBeepDuration = 0.144f; NumberOfWPM = 25; break;
            case 4: Difficulty = "Advanced Professional"; ShortBeepDuration = 0.04f; LongBeepDuration = 0.12f; NumberOfWPM = 30; break;
            case 5: Difficulty = "Expert"; ShortBeepDuration = 0.03f; LongBeepDuration = 0.09f; NumberOfWPM = 40; break;
            default: break;
        }
    }
    public void GetCustomFrequency(string frequency)
    {
        float.TryParse(frequency, out BeepFrequency);
    }
    public void ChooseBeepSystem(int BeepSystem)
    {
        switch(BeepSystem)
        {
            case 0: IsUsingFirstBeepSystem = true; break;
            case 1: IsUsingFirstBeepSystem = false; break;
        }
    }
    public void PlayMorseBeep(bool IsShort)
    {
        float Duration = IsShort ? ShortBeepDuration : LongBeepDuration;
        AudioSource.clip = CreateSineWaveClip(BeepFrequency, Duration);
        AudioSource.Play();
    }
    public void GetSpacingCharacter(int index)
    {
        if (index == 0)
        {
            SpacingCharacter = "/";
        }
        else
        {
            SpacingCharacter = " ";
        }
    }
    public void readOutputField()
    {
        if (OutputFieldDropdown.value == 1)
        {
            ConvertMorseCodeToAudio();
        }
    }
    public void ChooseIfTheActiveStateOfTheMorseVisualSignal(bool IsActive)
    {
        if (IsActive)
        {
            IsImageBeepPossible = true;
            BeepImage.SetActive(true);
        }
        else
        {
            IsImageBeepPossible = false;
            BeepImage.SetActive(true);
        }
    }
    //chatgpt
    AudioClip CreateSineWaveClip(float frequency, float duration)
    {
        if (IsUsingFirstBeepSystem == true)
        {
            int sampleRate = 44100; // Échantillonnage à 44.1 kHz
            int sampleCount = (int)(sampleRate * duration);
            AudioClip audioClip = AudioClip.Create("Beep", sampleCount, 1, sampleRate, false);

            float[] samples = new float[sampleCount];

            // Génère les échantillons de l'onde sinusoïdale
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * t); // Onde sinusoïdale
            }

            audioClip.SetData(samples, 0);
            return audioClip;
        }
        else
        {
            int sampleRate = 44100; // Échantillonnage à 44.1 kHz
            int sampleCount = (int)(sampleRate * duration);
            AudioClip audioClip = AudioClip.Create("Beep", sampleCount, 1, sampleRate, false);

            float[] samples = new float[sampleCount];

            float fadeDuration = 0.005f; // Durée du fade in/out (5 ms par exemple)
            int fadeSampleCount = (int)(fadeDuration * sampleRate);

            // Génère les échantillons de l'onde sinusoïdale
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float sample = Mathf.Sin(2 * Mathf.PI * frequency * t); // Onde sinusoïdale

                // Fade in (augmentation progressive de l'amplitude)
                if (i < fadeSampleCount)
                {
                    float fadeFactor = (float)i / fadeSampleCount;
                    sample *= fadeFactor;
                }
                // Fade out (réduction progressive de l'amplitude)
                else if (i > sampleCount - fadeSampleCount)
                {
                    float fadeFactor = (float)(sampleCount - i) / fadeSampleCount;
                    sample *= fadeFactor;
                }

                samples[i] = sample;
            }

            audioClip.SetData(samples, 0);
            return audioClip;
        }
    }
}