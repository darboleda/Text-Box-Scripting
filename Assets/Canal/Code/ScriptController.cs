using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


public class ScriptController : MonoBehaviour
{
    private struct CharacterProperties
    {
        public float DisplayTiming;
        public Color? Color;
        public AudioClip Sound;
    }

    public struct CharacterDisplayProperties
    {
        public Color? Color;
        public bool IsVisible;
    }

    private class ApplicationHelper : ICommandApplicationContext
    {
        private float defaultDelay;

        public ApplicationHelper(float defaultDelay)
        {
            this.defaultDelay = defaultDelay;
            DelayIncrement = null;
        }

        public Color? Color { get; set; }
        public int CharacterSoundClipIndex { get; set; }
        public float Delay { get; set; }
        public int PlaySoundIndex { get; set; }

        private float delayIncrement;
        public float? DelayIncrement
        {
            get { return delayIncrement; }
            set { delayIncrement = value ?? defaultDelay; }
        }

        public T GetVariableValue<T>(string variableName)
        {
            return (T)(object)(variableName + "::XXXXXXX");
        }

        public AudioClip PickCharacterSound(char c, AudioClip[] clips)
        {
            return char.IsWhiteSpace(c) ? null :
                   char.IsPunctuation(c) ? null : PickSound(CharacterSoundClipIndex, clips);
        }

        private AudioClip PickSound(int index, AudioClip[] clips)
        {
            if (index < 0 || index >= clips.Length) return null;
            return clips[index];
        }
    }

    private class ActivationHelper : ICommandActivationContext
    {
        private struct CharacterInfo : ICharacterInfo
        {
            public float Delay { get; set; }
        }


        private int currentIndex;
        private ScriptController owner;

        public ActivationHelper(ScriptController owner)
        {
            currentIndex = -1;
            this.owner = owner;
        }

        public ICharacterInfo this[int index]
        {
            get
            {
                return new CharacterInfo()
                {
                    Delay = owner.Properties[index].DisplayTiming * TimeUnit
                };
            }
        }

        public int LastDisplayedCharacterIndex
        {
            get { return currentIndex; }
        }

        public float TimeUnit { get { return owner.Script.TimeUnit; } }
        public float TimeScale { get { return owner.shouldSpeedUp ? owner.Script.SpeedUpTimeScale : 1; } }

        public int Advance()
        {
            currentIndex += 1;
            PlaySoundClip(owner.Properties[currentIndex].Sound);

            return currentIndex;            
        }

        public bool CheckEvent(string eventName)
        {
            switch (eventName)
            {
                case "ContinueAction":
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        owner.shouldSpeedUp = false;
                        return true;
                    }
                    return false;

                default:
                    return false;
            }
        }

        public AudioClip PlaySoundClip(int clipIndex)
        {
            return PlaySoundClip(owner.Script.Sounds[clipIndex]);
        }

        private AudioClip PlaySoundClip(AudioClip clip)
        {
            owner.ShouldPlaySound(clip);
            return clip;
        }

        public void SendEvent(string eventName)
        {
            //TODO send events to owner
            Debug.LogFormat("EVENT SENT: {0}", eventName);
        }
    }

    public TextWithEffects TextUi;
    public FormattedScript Script;

    [TextArea]
    [SerializeField]
    private string text;
    public string Text
    {
        get { return text; }
    }

    private CharacterProperties[] Properties;
    private ActivationHelper helper;

    public CharacterDisplayProperties? this[int index]
    {
        get
        {
            if (Properties == null || Mathf.Clamp(index, 0, Properties.Length - 1) != index) return null;

            var stored = Properties[index];
            var ret = new CharacterDisplayProperties()
            {
                Color = stored.Color,
                IsVisible = (helper != null ? helper.LastDisplayedCharacterIndex >= index : false)
            };
            return ret;
        }
    }

    public void OnEnable()
    {
        this.StartCoroutine(Activate());
    }

    public void OnDisable()
    {
        this.StopAllCoroutines();
    }

    private IEnumerator<ScriptCommand> commandEnumerator = null;
    public IEnumerator Activate()
    {
        List<ScriptCommand> commands;
        if (Script.Parse(out commands))
        {
            ApplyCommands(commands);
        }
        if (Text != TextUi.text) TextUi.text = Text;

        helper = new ActivationHelper(this);

        if (commandEnumerator != null) commandEnumerator.Dispose();
        commandEnumerator = commands.GetEnumerator();
        if (commandEnumerator.MoveNext())
        {
            yield return this.StartCoroutine(ContinueActivation());
        }
    }

    private IEnumerator ContinueActivation()
    {
        do
        {
            var command = commandEnumerator.Current;
            yield return StartCoroutine(command.Activate(helper));
        } while (commandEnumerator.MoveNext());
        commandEnumerator = null;
    }

    private AudioClip soundToPlay = null;
    private bool shouldSpeedUp = false;

    public void Update()
    {
        if (soundToPlay != null)
        {
            GetComponent<AudioSource>().PlayOneShot(soundToPlay);
            soundToPlay = null;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && commandEnumerator != null)
        {
            this.StopAllCoroutines();
            do
            {
                if (!commandEnumerator.Current.Skip(helper)) break;

            } while (commandEnumerator.MoveNext());
            this.StartCoroutine(ContinueActivation());
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            shouldSpeedUp = true;
        }
        if (Input.GetKeyUp(KeyCode.Return))
        {
            shouldSpeedUp = false;
        }
    }

    private void ShouldPlaySound(AudioClip clip)
    {
        soundToPlay = clip ?? soundToPlay;
    }

    public void ApplyCommands(IEnumerable<ScriptCommand> commands)
    {
        StringBuilder builder = new StringBuilder();
        List<CharacterProperties> properties = new List<CharacterProperties>();

        var context = new ApplicationHelper(Script.DefaultDelay);

        System.Action<char> DisplayCharacter = x =>
        {
            builder.Append(x);
            if (!char.IsWhiteSpace(x))
            {
                context.Delay += context.DelayIncrement.Value;
            }

            AudioClip actualSound = context.PickCharacterSound(x, Script.Sounds);
            properties.Add(new CharacterProperties()
            {
                DisplayTiming = context.Delay,
                Color = context.Color,
                Sound = actualSound,
            }
            );
        };

        foreach (var command in commands)
        {
            command.Apply(context);
            context.Delay = 0;
            foreach (var c in command.Evaluate(context))
            {
                DisplayCharacter(c);
            }
        }

        text = builder.ToString();
        Properties = properties.ToArray();
    }
}
