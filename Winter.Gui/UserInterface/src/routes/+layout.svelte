<script lang="ts">
import { allComponents, provideFluentDesignSystem } from '@fluentui/web-components';
import "../app.pcss";

provideFluentDesignSystem().register(allComponents);
</script>

<a href="http://localhost:5000/swagger">Swagger</a>
<slot />
