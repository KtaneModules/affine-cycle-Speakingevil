using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class AffineCycleScript : MonoBehaviour {

    public KMAudio Audio;
    public List<KMSelectable> keys;
    public GameObject[] dials;
    public TextMesh[] dialText;
    public TextMesh disp;

    private string[] message = new string[100] { "ADVANCED", "ADDITION", "ALLOCATE", "ALTERING", "BINARIES", "BILLIONS", "BULKHEAD", "BULLETED", "CIPHERED", "CIRCUITS", "COMPUTER", "CONTINUE", "DECRYPTS", "DIVISION", "DISCOVER", "DISPOSAL", "ENCIPHER", "ENTRANCE", "EQUATION", "EQUIPPED", "FINISHED", "FINDINGS", "FORTRESS", "FORWARDS", "GAUNTLET", "GAMBLING", "GATHERED", "GLOOMING", "HAZARDED", "HAZINESS", "HUNKERED", "HUNTSMAN", "INDICATE", "INDIGOES", "ILLUSION", "ILLUMINE", "JIGSAWED", "JIMMYING", "JUNCTION", "JUDGMENT", "KILOWATT", "KINETICS", "KNOCKOUT", "KNUCKLED", "LIMITING", "LINEARLY", "LINKAGES", "LABELING", "MONOGRAM", "MONOTONE", "MULTIPLY", "MULLIGAN", "NANOGRAM", "NANOTUBE", "NUMBERED", "NUMERALS", "OCTANGLE", "OCTUPLES", "OBSERVED", "OBSCURED", "PROGRESS", "PROJECTS", "POSITION", "POSITIVE", "QUADRANT", "QUADRICS", "QUICKEST", "QUINTICS", "REVERSED", "REVOLVED", "ROTATION", "RELATION", "STARTING", "STANDARD", "STOPPING", "STOPWORD", "TRIGGERS", "TRIANGLE", "TOGGLING", "TOGETHER", "UNDERRUN", "UNDERLIE", "ULTIMATE", "ULTRARED", "VICINITY", "VICELESS", "VOLTAGES", "VOLATILE", "WINGDING", "WINNABLE", "WHATEVER", "WHATNOTS", "YELLOWED", "YEASAYER", "YIELDING", "YOURSELF", "ZIPPERED", "ZIGZAGGY", "ZUGZWANG", "ZYMOGRAM"};
    private string[] response = new string[100] { "FORWARDS", "JIGSAWED", "HAZARDED", "NUMERALS", "BULLETED", "QUICKEST", "TOGETHER", "YOURSELF", "DISPOSAL", "HUNKERED", "ILLUSION", "MONOTONE", "FORTRESS", "STARTING", "VOLATILE", "GLOOMING", "KNUCKLED", "MULTIPLY", "ULTRARED", "BILLIONS", "NANOGRAM", "STANDARD", "YEASAYER", "QUINTICS", "PROJECTS", "JIMMYING", "KILOWATT", "QUADRANT", "POSITION", "LINEARLY", "ALTERING", "BINARIES", "OBSCURED", "LABELING", "JUNCTION", "VOLTAGES", "UNDERLIE", "COMPUTER", "INDICATE", "ZYMOGRAM", "ZUGZWANG", "CIPHERED", "MULLIGAN", "REVERSED", "HUNTSMAN", "NUMBERED", "POSITIVE", "ZIGZAGGY", "YELLOWED", "OCTUPLES", "GATHERED", "ROTATION", "CIRCUITS", "CONTINUE", "YIELDING", "BULKHEAD", "OBSERVED", "ILLUMINE", "EQUIPPED", "ALLOCATE", "STOPPING", "TRIGGERS", "LIMITING", "PROGRESS", "LINKAGES", "WHATEVER", "HAZINESS", "VICELESS", "TOGGLING", "MONOGRAM", "ENTRANCE", "FINISHED", "RELATION", "WINGDING", "ENCIPHER", "NANOTUBE", "QUADRICS", "DISCOVER", "DECRYPTS", "EQUATION", "OCTANGLE", "KNOCKOUT", "ZIPPERED", "UNDERRUN", "TRIANGLE", "WINNABLE", "ULTIMATE", "ADVANCED", "DIVISION", "WHATNOTS", "KINETICS", "VICINITY", "ADDITION", "GAUNTLET", "INDIGOES", "STOPWORD", "FINDINGS", "REVOLVED", "GAMBLING", "JUDGMENT" };
    private string[] ciphertext = new string[2];
    private string answer;
    private int[][] rot = new int[2][] { new int[8], new int[8]};
    private int pressCount;
    private bool moduleSolved;

    //Logging
    static int moduleCounter = 1;
    int moduleID;

    private void Awake()
    {
        moduleID = moduleCounter++;
        foreach(KMSelectable key in keys)
        {
            int k = keys.IndexOf(key);
            key.OnInteract += delegate () { KeyPress(k); return false; };
        }
    }

    void Start () {
        Reset();
	}

    private void KeyPress(int k)
    {
        keys[k].AddInteractionPunch(0.125f);
        if(moduleSolved == false)
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            if(k == 26)
            {
                pressCount = 0;
                answer = string.Empty;
            }
            else
            {
                pressCount++;
                answer = answer + "QWERTYUIOPASDFGHJKLZXCVBNM"[k];
            }
            disp.text = answer;
            if(pressCount == 8)
            {
                if(answer == ciphertext[1])
                {
                    moduleSolved = true;
                    Audio.PlaySoundAtTransform("InputCorrect", transform);
                    disp.color = new Color32(0, 255, 0, 255);
                }
                else
                {
                    GetComponent<KMBombModule>().HandleStrike();
                    disp.color = new Color32(255, 0, 0, 255);
                    Debug.LogFormat("[Affine Cycle #{0}]The submitted response was {1}: Resetting", moduleID, answer);
                }
                Reset();
            }
        }
    }

    private void Reset() {

        StopAllCoroutines();
        if (moduleSolved == false)
        {
            pressCount = 0;
            answer = string.Empty;
            int r = Random.Range(0, 100);
            string[] roh = new string[8];
            List<string>[] ciph = new List<string>[] { new List<string> { }, new List<string> { } };
            for (int i = 0; i < 8; i++)
            {
                dialText[i].text = string.Empty;
                rot[1][i] = rot[0][i];
                rot[0][i] = Random.Range(0, 7);
                if (rot[0][i] == 6)
                {
                    rot[0][i] = 7;
                }
                roh[i] = rot[0][i].ToString();
                ciph[0].Add("ZABCDEFGHIJKLMNOPQRSTUVWXY"[("ZABCDEFGHIJKLMNOPQRSTUVWXY".IndexOf(message[r][i]) * (2 * rot[0][i] + 1)) % 26].ToString());
                ciph[1].Add("ZABCDEFGHIJKLMNOPQRSTUVWXY"[("ZABCDEFGHIJKLMNOPQRSTUVWXY".IndexOf(response[r][i]) * (2 * rot[0][i] + 1)) % 26].ToString());
            }
            ciphertext[0] = string.Join(string.Empty, ciph[0].ToArray());
            ciphertext[1] = string.Join(string.Empty, ciph[1].ToArray());
            Debug.LogFormat("[Affine Cycle #{0}]The encrypted message was {1}", moduleID, ciphertext[0]);
            Debug.LogFormat("[Affine Cycle #{0}]The dial rotations were {1}", moduleID, string.Join(", ", roh));
            Debug.LogFormat("[Affine Cycle #{0}]The deciphered message was {1}", moduleID, message[r]);
            Debug.LogFormat("[Affine Cycle #{0}]The response word was {1}", moduleID, response[r]);
            Debug.LogFormat("[Affine Cycle #{0}]The correct response was {1}", moduleID, ciphertext[1]);
        }
        StartCoroutine(DialSet());
    }

    private IEnumerator DialSet()
    {
        int[] spin = new int[8];
        bool[] set = new bool[8];
        for (int i = 0; i < 8; i++)
        {
            if (moduleSolved == false)
            {
                spin[i] = rot[0][i] - rot[1][i];
            }
            else
            {
                spin[i] = - rot[0][i];
            }
            if(spin[i] < 0)
            {
                spin[i] += 8;
            }
        }
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (spin[j] == 0)
                {
                    if (set[j] == false)
                    {
                        set[j] = true;
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);
                        if (moduleSolved == false)
                        {
                            dialText[j].text = ciphertext[0][j].ToString();
                        }
                        else
                        {
                            switch (j)
                            {
                                case 0:
                                    dialText[j].text = "W";
                                    break;
                                case 2:
                                case 3:
                                    dialText[j].text = "L";
                                    break;
                                case 4:
                                    dialText[j].text = "D";
                                    break;
                                case 5:
                                    dialText[j].text = "O";
                                    break;
                                case 6:
                                    dialText[j].text = "N";
                                    break;
                                default:
                                    dialText[j].text = "E";
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    dials[j].transform.localEulerAngles += new Vector3(0, 0, 45);
                    spin[j]--;
                }
            }
            if (i < 7)
            {
                yield return new WaitForSeconds(0.25f);
            }
        }
        if(moduleSolved == true)
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
            GetComponent<KMBombModule>().HandlePass();
        }
        disp.text = string.Empty;
        disp.color = new Color32(255, 255, 255, 255);
        yield return null;
    }
#pragma warning disable 414
    private string TwitchHelpMessage = "!{0} QWERTY [Inputs letters] | !{0} cancel [Deletes inputs]";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*cancel\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            KeyPress(26);
        }
        else
        {
            var word = Regex.Match(command, @"^\s*([QWERTYUIOPASDFGHJKLZXCVBNM, ]+)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            if (!word.Success)
            {
                yield break;
            }
            yield return null;
            foreach (char letter in command)
            {
                KeyPress("QWERTYUIOPASDFGHJKLZXCVBNM".IndexOf(letter));
                yield return new WaitForSeconds(0.125f);
            }
        }
    }
}
