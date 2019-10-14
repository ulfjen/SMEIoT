import {useState} from "react";

const useUserCredentials = () => {
  const [username, setUsername] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [usernameErrors, setUsernameErrors] = useState<string>("");
  const [passwordErrors, setPasswordErrors] = useState<string>("");

  return {
    username, setUsername,
    password, setPassword,
    usernameErrors, setUsernameErrors,
    passwordErrors, setPasswordErrors
  }
};

export default useUserCredentials;
