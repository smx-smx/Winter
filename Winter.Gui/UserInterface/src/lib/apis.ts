import { PhotinoApi, WinterApi } from "./api";

const ASPNETCORE_BASE = 'http://localhost:5000';

export class Apis {
	private static _photinoApi: PhotinoApi|null = null;
	private static _winterApi: WinterApi|null = null;

	static get Photino(){
		if(this._photinoApi === null){
			this._photinoApi = new PhotinoApi({
				basePath: ASPNETCORE_BASE
			});
		}
		return this._photinoApi;
	}
	static get Winter(){
		if(this._winterApi === null){
			this._winterApi = new WinterApi({
				basePath: ASPNETCORE_BASE
			});
		}
		return this._winterApi;
	}
}
