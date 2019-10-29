import DiceAvatars from "@dicebear/avatars";
import sprites from "@dicebear/avatars-jdenticon-sprites";

export namespace SMEIoT {
  let avatarOptions = {};
  export let Avatars = new DiceAvatars(sprites(avatarOptions));
}

