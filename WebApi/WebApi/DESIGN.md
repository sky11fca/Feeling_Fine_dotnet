```markdown
# Design System Specification: The Liquid Glass Ethos

## 1. Overview & Creative North Star: "The Aquatic Prism"
The Creative North Star for this design system is **"The Aquatic Prism."** We are moving beyond flat, digital interfaces toward a tactile, hyper-realistic experience that mimics high-end physical glass submerged in deep blue water. 

This system rejects the "template" look of modern SaaS by embracing **intentional depth and organic fluidity**. We break the rigid grid through layered translucency, where elements don't just sit on a page—they float, refract light, and feel heavy yet ethereal. By utilizing high-contrast typography scales and overlapping "glass" containers, we create a signature editorial feel that is both authoritative and mesmerizingly beautiful.

---

## 2. Colors & Surface Philosophy
The palette is rooted in deep, vibrant blues (`primary: #7fafff`) and rich, midnight neutrals (`surface: #0e0e10`). 

### The "No-Line" Rule
**Explicit Instruction:** Designers are prohibited from using 1px solid borders for sectioning or containment. 
*   **The Law:** Boundaries must be defined solely through background color shifts or tonal transitions. 
*   **Implementation:** Use a `surface-container-low` section sitting directly on a `surface` background. The shift in tone is the boundary.

### Surface Hierarchy & Nesting
Treat the UI as a series of physical layers—like stacked sheets of frosted sapphire glass.
*   **Base:** `surface` (#0e0e10).
*   **Level 1 (Sections):** `surface-container-low` (#131315).
*   **Level 2 (Cards/Containers):** `surface-container` (#19191c).
*   **Level 3 (Floating Modals):** `surface-container-highest` (#252528).

### The "Glass & Gradient" Rule
To achieve the "Liquid" feel, floating elements must use **Glassmorphism**. 
*   **Formula:** Apply a semi-transparent `surface-variant` with a `backdrop-filter: blur(20px)`. 
*   **Signature Glow:** Use a subtle `inner-shadow` or a 1px `outline-variant` at 15% opacity to mimic the "catchlight" on the edge of a glass pane.
*   **Soulful Gradients:** Main CTAs should transition from `primary` (#7fafff) to `primary-container` (#64a1ff) at a 135-degree angle to simulate light passing through a liquid volume.

---

## 3. Typography: Editorial Modernism
We use **Inter** as our typographic backbone. The hierarchy is designed to feel like a high-end tech journal: bold, spacious, and deliberate.

*   **Display (lg/md):** Reserved for hero moments. Use `display-lg` (3.5rem) with tight letter-spacing (-0.02em) to create a "heavy" visual anchor.
*   **Headlines:** `headline-lg` (2rem) serves as the primary entry point for content blocks.
*   **Body:** `body-lg` (1rem) for readability. Ensure a line-height of 1.6 to maintain the "breathing room" required by the aesthetic.
*   **Contrast:** Pair `on-surface` (white/near-white) for headings with `on-surface-variant` (#acaaad) for body text to create a sophisticated tonal hierarchy that reduces eye strain.

---

## 4. Elevation & Depth: Tonal Layering
Traditional drop shadows are too "dirty" for a liquid glass aesthetic. We use **Ambient Light Simulation**.

*   **The Layering Principle:** Depth is achieved by "stacking." A `surface-container-lowest` card placed on a `surface-container-low` section creates a natural "recessed" or "lifted" look without a single pixel of shadow.
*   **Ambient Shadows:** For floating elements, use a "Large & Diluted" shadow: `box-shadow: 0 20px 40px rgba(0, 93, 185, 0.08)`. Notice the shadow is tinted with `inverse-primary` blue, not black, to simulate light refracting through the liquid.
*   **The Ghost Border Fallback:** If a container requires further definition, use the `outline-variant` token at **15% opacity**. Never use 100% opaque lines.
*   **Refractive Edge:** Apply a 1px top-border (inset) using `primary-fixed` at 20% opacity to simulate the way glass catches overhead light.

---

## 5. Components: Physical Glass Primitives

### Buttons (Liquid Capsules)
*   **Primary:** A gradient of `primary` to `primary-dim`. Corner radius: `full`. Include a subtle inner glow on the top edge.
*   **Secondary:** Glassmorphic. `surface-variant` at 40% opacity with `backdrop-filter: blur(10px)`.
*   **States:** On hover, increase the `backdrop-blur` and slightly brighten the `surface-tint`.

### Input Fields (Recessed Wells)
*   Fields should look like they are carved *into* the glass. 
*   **Styling:** Use `surface-container-lowest` with an inner shadow. Radius: `md` (1.5rem). 
*   **Active State:** The border doesn't just change color; it glows using the `tertiary` (#68d3ff) token at low opacity.

### Cards & Lists (Seamless Flow)
*   **Strict Rule:** No divider lines. Use `spacing-6` (2rem) of vertical white space to separate items.
*   **Interaction:** On hover, a card should shift from `surface-container` to `surface-container-high`, creating a "liquid rise" effect.

### Chips (Sea Pebbles)
*   Small, organic shapes using `full` roundedness. 
*   Use `secondary-container` for the background to provide a sophisticated purple-blue contrast against the primary blue theme.

---

## 6. Do’s and Don’ts

### Do:
*   **Use Asymmetry:** Place a large `display-md` heading off-center to create an editorial, premium feel.
*   **Embrace Translucency:** Allow background gradients or shapes to be partially visible through your UI panes.
*   **Smooth Radii:** Use `xl` (3rem) for large containers and `full` for buttons. Sharp corners kill the liquid illusion.

### Don’t:
*   **Don't use pure black:** Use `surface` (#0e0e10) to keep the depth "inky" rather than "empty."
*   **Don't use 1px dividers:** They shatter the glass metaphor. Use tonal shifts or white space.
*   **Don't over-saturate:** While the blues are vibrant, use `on-surface-variant` for secondary text to keep the interface professional and readable.
*   **Don't forget the blur:** Glassmorphism without `backdrop-blur` is just a low-opacity box. The blur is what provides the "liquid" density.

---

## 7. Spacing & Rhythm
The system relies on generous breathing room to feel "High-End." 
*   **Standard Padding:** Use `spacing-6` (2rem) for internal card padding.
*   **Section Gaps:** Use `spacing-16` (5.5rem) or `spacing-20` (7rem) between major content groups to allow the "Liquid Glass" elements to occupy their own space without clutter.```