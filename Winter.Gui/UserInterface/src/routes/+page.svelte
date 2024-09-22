<script lang="ts">
import { Apis } from '$lib/apis';
import { Button } from 'flowbite-svelte';

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
	<Button on:click={() => startCbsSession()}>Start CBS session</Button>
	{windirPath}
</div>

<ul>
{#each components as cmp}
	<li>{cmp}</li>
{/each}
</ul>