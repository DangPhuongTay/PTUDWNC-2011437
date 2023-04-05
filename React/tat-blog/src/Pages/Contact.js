import React, { useEffect } from 'react';
const Index = () => {
useEffect(() => {
document.title = 'Liên hệ';
}, []);
return (
<h1>
Đây là trang liên hệ
</h1>
);
}
export default Index;