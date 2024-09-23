<script lang="ts">
	import { Apis } from '$lib/apis';

let windirPath = '';
let components:string[] = [];

async function startCbsSession(){
	const photino = Apis.Photino;
	windirPath = (await photino.showOpenFolder('Choose Windows Directory')).data;
	const root = (await photino.getRootPath(windirPath)).data;
	
	const res = (await Apis.Winter.startCbsSession({
		bootDrive: root,
		winDir: windirPath
	})).data;

	components = (await Apis.Winter.getComponents(res)).data;

	console.log(res);

}
</script>

<div>
	<fluent-button appearance="accent" on:click={() => startCbsSession()}>Start CBS session</fluent-button>
	{windirPath}
</div>

<fluent-tree-view>
{#each components as cmp}
	<fluent-tree-item>{cmp}</fluent-tree-item>
{/each}
</fluent-tree-view>