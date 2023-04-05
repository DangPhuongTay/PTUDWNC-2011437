import React, { useEffect } from 'react';
const Index = () => {
useEffect(() => {
document.title = 'RSS';
}, []);
return (
<h1>
Đây là trang RSS
</h1>
);
}
export default Index;