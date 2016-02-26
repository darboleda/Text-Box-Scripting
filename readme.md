# Text Box Scripting

Do text box things like

* Make letters appear one after the other with sounds!
* Include pauses!
* Change colors!
* And not much more!

Also supports a few text transformation effects not untirely related to the
whole scripting thing.

## Script commands
Commands with the following syntax:

    @[command];
    @[command]([command args])
    
Where [command] is the command name. Arguments are delimited by spaces, and there
is currently not support for quoted strings with spaces as arguments. If there are
no parameters, you should probably use the `@[command];` version.

To display the `@` character, just enter two in a row (i.e. `@@`).

* ##### @n;
  Displays a newline. Equivalent to having a blank line in the script.

* ##### @v | @var | @variable (name)
  Display the variable `name`'s value as a string. Variable is defined in scripting context.

* ##### @p | @pause (timeout [skippable = true])
  Pause the script interpretation for `timeout` units. If `skippable` is true, you can
  skip through this by pressing the skip button (`Escape`).

* ##### @sfx (soundIndex [timeout])
  Play the audio clip at `soundIndex`. If `timeout` is provided, wait that many units after
  the sound begins before continuing. Currently no support for waiting until the sound
  finishes playing automatically.

* ##### @e | @event (eventName [delay] [skippable = true])
  Send the context the event `eventName` after `delay` units. If `skippable` is true, if the
  text is skipped, the event will not be sent.

* ##### @c | @color [(color)]
  Sets the text color. Colors use the following hex formats `#RGB`, `#RGBA`, `#RRGGBB`, and
  `#RRGGBBAA`. Using the command without arguments (e.g. `@c;`) will reset the color to the
  default color of the text box.

* ##### @s | @speed [(delay)]
  Sets the text speed to `delay` units between each character appearing. Using the command
  without arguments (e.g. `@s;`) will reset the text speed to the default.

* ##### @a | @audio [(clipIndex)]
  Sets the character sound index to `clipIndex`. Using the command without arguments (e.g.
  `@a;`) will reset the audio to the default.

* ##### @w | @wait [(eventName)]
  Wait for the event `eventName` from the context before continuing. Using the command with
  no arguments (e.g. `@w;`) will wait for the "ContinueAction" event, which is sent by the
  context when the `Return` key is pressed.

## Components
Use these components to get all this stuff working.

* ##### ScriptController
  Binds the **FormattedScript** with the **TextWithEffects**. Interprets the script formatting
  and applies it. This includes playing the sound associated with displaying the characters.

* ##### TextWithEffects
  A subclass of **UnityEngine.UI.Text** that applies **TextEffect**s to the text mesh.

* ##### AppearingText
  A **TextEffect** that actually hides the characters that the **ScriptController** has not yet
  marked visible.

* ##### ColoredText
  A **TextEFfect** that changes the colors of the text based on the commands as interpreted
  by the **ScriptController**.
  
* ##### PerlinText
  A **TextEffect** which introduces Perlin-based text wobbling. Parameters include offsetting,
  multipliers, speed, and AnimationCurves to map the Perlin noise return value to other values.
