import {useState} from "react";

const useUserCredentials = () => {
  const [userName, setUserName] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [userNameErrors, setUserNameErrors] = useState<string>("");
  const [passwordErrors, setPasswordErrors] = useState<string>("");

  return {
    userName, setUserName,
    password, setPassword,
    userNameErrors, setUserNameErrors,
    passwordErrors, setPasswordErrors
  }
};

export default useUserCredentials;
