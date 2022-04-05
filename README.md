Dice-Roller
===
WotR-Edition
----

An alternative way to create stats. Changes the point buy system to a dice roll system.

Use
----
in UMM-Menu:
1. activate/deactivate Mod to change between Point Buy and StatRoll 
2. reroll shown Stats with the reroll button, current ways to roll are on the left.

in Attribute Allocator Window:
1. The right button switches current stat with next stat in List.
2. Points shows the total point buy value of all stats.
3. The buttons below are self-explanatory.

Dice-Expressions
----
The Mod uses a Parser which supports parantheses and all basic math(*, -, +) except division (not useful with integers)
It checks for garbage input (throws 1's if it does not understand the expression), but it is probably not failproof.

The dice expression follows this pattern: 
```Xd[MIN,MAX]r[R1,R2,...]k{h/l}Z```
where `X`, `MIN`, `MAX`, `R1`, `R2`, ..., `Z` can be random values or calculations in parantheses also possibly containing other dice expressions.

Here is an overview of the parameters:
- `X` is the amount of dice rolled (Mandatory)
- `MIN` is the minimum value of a thrown dice (Optional) 
- `MAX` is the maximum value of a thrown dice (Mandatory)
- `R1`, `R2`, ... are values to be rerolled (Optional)
- `Z` is the amount of dice to be kept, either highest (h) or lowest (l) (Optional)

Some examples would be:
- `3d[2,8]r[3,5,7]kh2+6` Translates to: roll 3 dice from 2 to 8, reroll odd numbers, keep highest 2 and add 6.
- `5d[8]kl4-2` Translates to: roll 5 dice from 1 to 8, keep lowest 4 and subtract 2.
- `(1d[4]+1)d[8]` Translates to: roll 1 dice from 1 to 4, add 1 and use this as the amount of dice from 1 to 8.
- `18` Translates to: use 18.

There are more examples and standard stat array types to be found in the mod itself, just try them out.

Known-Issues:
----
1. if Intelligence is changed below 3, the background-option will disappear, if you change it to a higher value, it will reappear, but without selection. Fix: just reselect it.
2. None.

To-Do:
----
1. Make it so you can disable Mod and the game switches to point buy with rolled Stats as base.
2. Support switching upwards and downwards.
3. ... ?

Installation
----
1. Install [Unity Mod Manager](https://www.nexusmods.com/site/mods/21) and point it at your Pathfinder: WotR game directory (if using Steam, this will typically be at: `C:\Program Files (x86)\Steam\steamapps\common\Pathfinder Second Adventure\`
2. Use UMM to install the mod by dragging and dropping the zip file into its Mods tab.

Unistallation
----
1. Use UMM to uninstall the mod.  Or delete the mod from the Pathfinder WotR\Mods directory.

Credits
----
v1ld for the Mod: [Custom Map Markers](https://www.nexusmods.com/pathfinderkingmaker/mods/131) - Your Mod inspired me to actually publish something I created. Thanks.
