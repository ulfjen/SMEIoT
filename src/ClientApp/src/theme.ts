import { PaletteOptions, Palette } from "@material-ui/core/styles/createPalette";
import { TypographyOptions } from "@material-ui/core/styles/createTypography";

// https://material.io/resources/color/#!/?view.left=1&view.right=0&secondary.color=42A5F5&primary.color=5C6BC0
const palette: PaletteOptions = {
  primary: {
    main: "#5c6bc0",
    light: "#8e99f3",
    dark: "#26418f",
    contrastText: "#ffffff"
  },
  secondary: {
    main: "#42a5f5",
    light: "#80d6ff",
    dark: "#0077c2",
    contrastText: "#000000"
  }
};

const typography: TypographyOptions | ((palette: Palette) => TypographyOptions) = {
  h1: {
    fontSize: "6rem"
  },
  h2: {
    fontSize: "3.75rem"
  },
  h3: {
    fontSize: "3rem"
  },
  h4: {
    fontSize: "2.125rem"
  },
  h5: {
    fontSize: "1.5rem"
  },
  h6: {
    fontSize: "1.25rem"
  }
}

export { palette, typography };
