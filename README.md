SpicyTaco.MultiStyle
==================

Compose your WPF styles just like in CSS.

[My source of inspiration](http://web.archive.org/web/20101125040337/http://bea.stollnitz.com/blog/?p=384)

Why not just use that code? It works, right?

Sort of. When you run the code everything works as expected. Unfortunately, the designer is a different story. Controls look as if there are no styles applied.

With the attached property you get pretty good designer support. I say pretty good because it does not update when you change one of the styles properties without a build. You could also make a change to the attached property, wait for the designer to catch up, then change it back.

Usage Examples
--------------

Given a set of styles such as these

```XML
<Style x:Key="BigText" TargetType="TextBlock">
    <Setter Property="FontSize" Value="36"/>
</Style>
<Style x:Key="CenteredText" TargetType="TextBlock">
    <Setter Property="VerticalAlignment" Value="Center"/>
    <Setter Property="HorizontalAlignment" Value="Center"/>
</Style>
<Style x:Key="GreenText" TargetType="TextBlock">
    <Setter Property="Foreground" Value="Green"/>
</Style>
<Style x:Key="PurpleText" TargetType="TextBlock">
    <Setter Property="Foreground" Value="Purple"/>
</Style>
```

You can combine these styles into a merged style and set it to a control like so

```XML
<TextBlock Text="MultiStyle Sample App" 
           st:Multi.Styles="BigText CenteredText GreenText PurpleText"/>
```

This results in

<img src="example_screen_shot.PNG" alt="Icon" style="width: 128px; height: 128px;"/><br/>

Credits
-------

<img src="icon/icon_61620.png" alt="Icon" style="width: 128px; height: 128px;"/><br/>
[Furious designed by Matt Brooks from the Noun Project](http://thenounproject.com/Mattebrooks/icon/61620/)
