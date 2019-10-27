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

    private int r;
    private string[] message = new string[100] { "ADVANCED", "ADDITION", "ALLOCATE", "ALLOTTED", "BINARIES", "BILLIONS", "BULKHEAD", "BULWARKS", "CIPHERED", "CIRCUITS", "COMPUTER", "COMPILER", "DECRYPTS", "DIVISION", "DISCOVER", "DISCRETE", "ENCIPHER", "ENTRANCE", "EQUATION", "EQUALISE", "FINISHED", "FINDINGS", "FORTRESS", "FORTUNES", "GAUNTLET", "GAMBLING", "GATHERED", "GATEWAYS", "HAZARDED", "HAZINESS", "HUNKERED", "HUNGRIER", "INDICATE", "INDIGOES", "ILLUSION", "ILLUDING", "JIGSAWED", "JIMMYING", "JUNCTION", "JUNCTURE", "KILOWATT", "KINETICS", "KNOCKOUT", "KNOWABLE", "LIMITING", "LINEARLY", "LINKAGES", "LINGERED", "MONOGRAM", "MONOTONE", "MULTIPLY", "MULCTING", "NANOGRAM", "NANOTUBE", "NUMBERED", "NUMERATE", "OCTANGLE", "OCTUPLES", "OBSERVED", "OBSTACLE", "PROGRESS", "PROJECTS", "POSITION", "POSITRON", "QUADRANT", "QUADRICS", "QUICKEST", "QUITTERS", "REVERSED", "REVOLVED", "ROTATION", "RELATIVE", "STARTING", "STANDARD", "STOPPING", "STOCCATA", "TRIGGERS", "TRIANGLE", "TOMOGRAM", "TOMORROW", "UNDERRUN", "UNDERLIE", "ULTIMATE", "ULTRAHOT", "VICINITY", "VICELESS", "VOLTAGES", "VOLUMING", "WINGDING", "WINNABLE", "WHATEVER", "WHATSITS", "YELLOWED", "YEASAYER", "YIELDERS", "YOURSELF", "ZIPPERED", "ZIGZAGGY", "ZUGZWANG", "ZYMOGENE"};
    private string[] response = new string[100] { "UNDERRUN", "HAZARDED", "ENTRANCE", "ULTIMATE", "WINGDING", "YEASAYER", "YIELDERS", "JIGSAWED", "JUNCTURE", "JUNCTION", "LINEARLY", "VICINITY", "OCTANGLE", "JIMMYING", "TRIGGERS", "ILLUDING", "BULKHEAD", "BULWARKS", "TRIANGLE", "FORTUNES", "HUNGRIER", "ROTATION", "POSITION", "WINNABLE", "REVERSED", "ULTRAHOT", "QUADRICS", "OCTUPLES", "ZIPPERED", "RELATIVE", "KILOWATT", "BILLIONS", "STOPPING", "PROJECTS", "QUITTERS", "FORTRESS", "NUMERATE", "VOLUMING", "DISCRETE", "ZUGZWANG", "STOCCATA", "FINISHED", "MULTIPLY", "ENCIPHER", "MONOTONE", "COMPILER", "QUICKEST", "MULCTING", "LIMITING", "NUMBERED", "ADDITION", "YOURSELF", "DISCOVER", "DECRYPTS", "ZIGZAGGY", "STARTING", "KINETICS", "KNOCKOUT", "PROGRESS", "NANOGRAM", "GAMBLING", "QUADRANT", "REVOLVED", "DIVISION", "NANOTUBE", "TOMOGRAM", "VOLTAGES", "CIPHERED", "HAZINESS", "HUNKERED", "MONOGRAM", "FINDINGS", "STANDARD", "UNDERLIE", "OBSERVED", "GAUNTLET", "ALLOCATE", "OBSTACLE", "POSITRON", "ADVANCED", "EQUALISE", "VICELESS", "GATEWAYS", "LINKAGES", "INDICATE", "KNOWABLE", "ILLUSION", "LINGERED", "GATHERED", "EQUATION", "ZYMOGENE", "BINARIES", "INDIGOES", "ALLOTTED", "WHATSITS", "TOMORROW", "WHATEVER", "YELLOWED", "COMPUTER", "CIRCUITS" };
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
            r = Random.Range(0, 100);
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
    private string TwitchHelpMessage = "!{0} QWERTYUI [Inputs letters] | !{0} cancel [Deletes inputs]";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {

        if (command.ToLowerInvariant() == "cancel")
        {
            KeyPress(26);
            yield return null;
        }
        else
        {
            command = command.ToUpperInvariant();
            var word = Regex.Match(command, @"^\s*([A-Z\-]+)\s*$");
            if (!word.Success)
            {
                yield break;
            }
            command = command.Replace(" ", string.Empty);
            foreach (char letter in command)
            {
                KeyPress("QWERTYUIOPASDFGHJKLZXCVBNM".IndexOf(letter));
                yield return new WaitForSeconds(0.125f);
            }
            yield return null;
        }
    }
}
