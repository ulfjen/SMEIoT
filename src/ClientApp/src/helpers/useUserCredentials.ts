import { useState } from "react";

export interface UserCredentials {
  userName: string;
  password: string;
  userNameError: string;
  passwordError: string;
  setUserName: React.Dispatch<React.SetStateAction<string>>;
  setPassword: React.Dispatch<React.SetStateAction<string>>;
  setUserNameError: React.Dispatch<React.SetStateAction<string>>;
  setPasswordError: React.Dispatch<React.SetStateAction<string>>;
  entityError: string;
  setEntityError: React.Dispatch<React.SetStateAction<string>>;
}

function useUserCredentials(): UserCredentials {
  const [userName, setUserName] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [userNameError, setUserNameError] = useState<string>("");
  const [passwordError, setPasswordError] = useState<string>("");
  const [entityError, setEntityError] = useState<string>("");

  return {
    userName, setUserName,
    password, setPassword,
    userNameError, setUserNameError,
    passwordError, setPasswordError,
    entityError, setEntityError
  }
};

export default useUserCredentials;
