import createMuiTheme from '@material-ui/core/styles/createMuiTheme';
import red from '@material-ui/core/colors/red';

// https://material.io/resources/color/#!/?view.left=1&view.right=0&secondary.color=F3E5F5&primary.color=42A5F5&secondary.text.color=424242
const theme = createMuiTheme({
  palette: {
    primary: {
      main: "#42a5f5",
      light: "#80d6ff",
      dark: "#0077c2",
      contrastText: "#000000"
    },
    secondary: {
      main: "#f4e5f5",
      light: "#ffffff",
      dark: "#c0b3a2",
      contrastText: "#424242"
    },
    error: {
      main: red.A400,
    },
    background: {
      default: '#fff',
    },
  },
});

export default theme;
