using System.Collections;
using System.Linq;
using UnityEngine;

public class Spiderman2004 : MonoBehaviour {

   public KMBombInfo Bomb;
   private KMAudio.KMAudioRef SoundIThink;
   public KMAudio Audio;

   public KMSelectable mod;

   public KMSelectable Beeg;

   int clicks = 0;
   bool focused = false;
   bool highlighted = false, solved = false;

   static int ModuleIdCounter = 1;
   int ModuleId;

   void Awake () {
        ModuleId = ModuleIdCounter++;

        Beeg.OnInteract += delegate () { BeegPress(); return false; };

        mod.OnHighlight += delegate () { MusicStarter(); highlighted = true; };
        mod.OnHighlightEnded += delegate () { MusicEnder(); highlighted = false; };
        mod.OnFocus += delegate () { focused = true; };
        mod.OnDefocus += delegate () { MusicEnder(); focused = false; };

        if (Application.isEditor) {
            focused = true;
        }
   }

   void MusicStarter () {
        if (SoundIThink == null) {
            SoundIThink = Audio.PlaySoundAtTransformWithRef("Pain", transform);
        }
   }

   void MusicEnder () {
        if (SoundIThink != null && !focused) {
            SoundIThink.StopSound();
            SoundIThink = null;
        }
   }

   void BeegPress () {
        Beeg.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Beeg.transform);
        clicks++;
        Audio.PlaySoundAtTransform("Pizza Time", Beeg.transform);
        //Debug.LogFormat("[Spiderman 2004 #{0}] {1}", ModuleId, clicks);
        if (clicks == 2004) {
            solved = true;
            Debug.LogFormat("[Spiderman 2004 #{0}] You have pressed it {1} time(s). Go on. Make my day.", ModuleId, clicks);
            GetComponent<KMBombModule>().HandlePass();
        }
   }

   void Start () {
        Debug.LogFormat("[Spiderman 2004 #{0}] Big Mistake.", ModuleId);
   }

   void Update () {
        if (Input.GetKeyDown(KeyCode.ScrollLock)) {
            BeegPress();
        }
        if (SoundIThink != null && !focused && !highlighted) {
            SoundIThink.StopSound();
            SoundIThink = null;
        }
        if (SoundIThink == null && (focused || highlighted)) {
            MusicStarter();
        }
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = "Use \"!{0} press\" to press the button once, or \"!{0} press 2004\" to press the button 2004 times very quickly.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
        var CmdUpper = Command.Trim().ToUpperInvariant();
        yield return null;
        if (CmdUpper.StartsWith("PRESS ")) {
            var val = CmdUpper.Split().Skip(1).Join(" ");
            var pressCnt = 1;
            if (int.TryParse(val, out pressCnt))
            {
                yield return null;
                for (var x = 0; x < pressCnt && !solved; x++)
                {
                    Beeg.OnInteract();
                    yield return string.Format("trycancel Canceled pressing the button after {0} press(es)! Why would you do that?", x + 1);
                }
            }
            else
                yield return "sendtochaterror Big mistake. Was that a typo on your press command?";
        }
        else if (CmdUpper == "PRESS")
        {
            yield return null;
            Beeg.OnInteract();
        }
        else
        {
            //clicks = 0;
            yield return "sendtochaterror Big mistake. Was that a typo?";
        }
   }

   IEnumerator TwitchHandleForcedSolve () {
        for (int i = 0; i < 2004 - clicks; i++) {
            Beeg.OnInteract();
            yield return new WaitForSeconds(.001f);
        }
   }
}
