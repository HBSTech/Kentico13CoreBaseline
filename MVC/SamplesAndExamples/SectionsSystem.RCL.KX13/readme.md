
# Sections
Many website contain horizontal areas, "sections"

Often a distinction can also be made between the **style** of a section, and the **content** of the section. 

You may have a carousel (content) with a white background (style), or with a picture background (style)

This Sample shows an implementation of a Styled Section System

# Section Styling (Surrounding)
Many web templates offer a variety of section styling.  This sample shows 5 different variations:

* Default (alternating, since often you have white/gray alternating patterns)
* Color (Solid color, based on the theme)
* Image (Image Background)
* Parallax (Parallax Image Background)
* Video (Video Background)

Additionally, there are Themes (mainly the coloring of the background for Color styles, and/or button coloring for others), and Contrast options.  These may or may not be supported by your theme, so you may exclude them when you actually implement your own system.

These options are defined in Kentico on a `Section - Inherited Class` page type, and store their values in the NodeCustomData using the [Xperience Page Custom Data Control Extender](https://github.com/wiredviews/xperience-page-custom-data-control-extender).  You can take a look at the `SectionInheritedClass.cs` for examples of the model.

## Themes
Themes determine the color of the background.  Since the text must be either white/black depending on the lightness of the color, colors should be defined and the proper CSS generated based on the color.  I would recommend using CSS Variables to apply different variable values based on the color.

## Contrasts
There are 3 options for contrast presented, although implementing them you will need to do yourself.

1. None
2. Over Text (Text is contrasted, usually done through a drop shadow)
3. Over Section (semi-transparent white/black tint is applied over the background of the section, allowing text to stand out)

# Section Models (MVC)

## ISection / Section
ISection is the section interface that provides methods to retrieve all the core elements that make up the Section wrapping.  It's implemented only by the `Section` class, but the `Section` can be typed, so an interface to unite them is needed.

## Section
The Section class contains all the core elements for the given section styling, plus an additional `SectionModel` which is where you can provide section content specific properties.

For example, there are (at time of writing this) a WidgetSection, GeneralContentSection, and FeatureSection.  These different section content types contain different properties and are set on the SectionModel.

## Basic Elements
Most content can be broken down into basic elements that build other components.  These models allow you to map your data into a combination of these individual components, which can then be rendered more easily.

### IVisualItem
Represents a visual element.  There are...
1. `IconItem` (Icon usually sourced from a font or svg library)
2. `ImageItem` (Image with a file Url)

It is recommended that you unify rendering through a Tag Helper and pass your model to it.

### ILink
Represents a hyperlink, there are...
1. `ButtonLink` (Represents a button styled link)
2. `TextLink` (Represents a text hyperlink) 
3. `GeneralLink` (Represents a link that should be applied to a whole section/entity)

It is recommended that you unify rendering through a Tag Helper and pass your model to it.

There is a `bl-link-wrapper` attribute and tag helper that allow you to wrap divs and other elements with an `ILink` , however all other rendering of links will be very much theme-specific.

### IVideoSection
Represents video content, there are...
1. `Html5VideoSection` (allows mp4, ogg, or webm urls)
1. `YoutubeVideoSection` (youtube sourced video)

A sample `<bs-video-element-display>` tag helper has been provided in `IVideoElementDisplay`, however you may need to implement your own tag helper.

### ContentItem
Has a Header, Sub Header, and Content HTML.  

It is recommended that you unify rendering through a Tag Helper and pass your model to it.

## BasicSectionItem
This is an optional model to use for individual items in your section's content rendering, and contains an array of ContentItem, IVideoSection, IVisualItem, and ILink.

Optionally you can also include a specific model to contain 'other' properties for your section item.

# Section View Components
There are two view components to leverage for Sections:

`<vc:sections x-parent-page=@currentPageIdentity />` which will get all sections within the child `Sections` folder, or those in the `Sections` Page Relationship, and then call `<vc:render-section x-section=@iSectionModel/>`

`<vc:render-section x-section=@iSectionModel/>` / `<vc:render-section x-section-identity=@theSectionIdentity />`, this View component will render the section itself with styling, and contains the caching logic.

# Creating a new Section
When building out a new Section, follow these steps:

1. Create a new Section Page type (xxx.Section_____)
2. Inherit from the `Section - Inherited Class`.
3. Add in any properties that are used to build your section (be sure to prefix the properties with a unique prefix to prevent sql issues when joining across differnet sections)**.
4. Generate and Save your Page Type class.
5. Create a Kentico Agnostic version of your new Section Type (possibly leveraging the base Content models)
6. Modify the SectionRepository.GetSectionsAsync (if your section can be within general sections), or create a new method to retrieve your content.
  * This usually involves adding your section page type to the queries, and modifying the TreeNodeToSection method

** These properties should pertain to the section itself, but you will probably need/have sub-items that will build out variable content within it.  For example, the Featured Section's page type has a Header and then allows child 'Basic Element - Compound Content' that represent each Feature.

If your section is a "General" section (that can be combined with other sections)
1. Modify the `SectionContent.cshtml` file, adding an else if on your model type to call a view component
2. Create your view component to receive the Section<YourType> section and do any additional logic to render (retrieve sub items, etc)

## Questions
If you have questions, hit me up at tfayas @ gmail.com 