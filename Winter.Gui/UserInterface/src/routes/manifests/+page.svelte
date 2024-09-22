<script lang="ts">
    import { onMount } from "svelte";

	const BIND_OBJECT_NAME = "winter";

	let items:any[] = [];

	async function initialize(){
		await window.CefSharp.BindObjectAsync(BIND_OBJECT_NAME);
		window.winter.initialize();
		items = await window.winter.getAllManifests();
	}

	onMount(async () => {
		initialize();
	});

	import VirtualList from '@sveltejs/svelte-virtual-list';
</script>

<div class="w-full h-full">
{#key items}
<VirtualList items={items} let:item>
	<p>{item}</p>
</VirtualList>
{/key}
</div>