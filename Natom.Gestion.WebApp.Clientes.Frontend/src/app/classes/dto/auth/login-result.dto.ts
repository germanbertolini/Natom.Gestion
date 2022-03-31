import { UserDTO } from "../user.dto";

export class LoginResult {
    public user: UserDTO;
    public token: string;
    public permissions: Array<string>;
}