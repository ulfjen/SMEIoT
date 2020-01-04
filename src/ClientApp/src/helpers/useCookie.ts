import Cookies from 'js-cookie';

export function useCookie(key: string): any {
  const val = Cookies.get(key);

  if (!val) { return; }

  if (["yes", "y", "1"].includes(val)) { return true; }
  if (["no", "n", "0"].includes(val)) { return false; }

  try {
    return JSON.parse(val);
  } catch {
    return;
  }
}

export class AppCookie {
  userName?: string;
  admin: boolean = false;
};

export function useAppCookie(): AppCookie {
  const u = useCookie("currentUser");
  let cookie = new AppCookie();
  if (u) {
    if (u.hasOwnProperty("userName")) { cookie.userName = u.userName; }
    if (u.hasOwnProperty("admin")) { cookie.admin = u.admin; }
  }
  return cookie;
}
