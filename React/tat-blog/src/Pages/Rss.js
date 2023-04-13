import React, { useEffect } from 'react';
const Index = () => {
useEffect(() => {
document.title = 'Rss';
}, []);
return (
<h1>
Đây là trang RSS
</h1>
);
}
export default Index;