import { useState, useEffect } from "react";
import { isEmptyOrSpaces } from "../../Utils/Utils";
import Table from "react-bootstrap/Table";
import { getAuthors } from "../../Services/Widgets";
import { format } from 'date-fns'


const AuthorsList = () => {
     
  const [AuthorList, setAuthorList] = useState([]);

  useEffect(() => {
    getAuthors().then((data) => {
      if (data) setAuthorList(data);
      else setAuthorList([]);
    });
  }, []);
  
  return (
    <>
     <Table striped responsive bordered>
           
            <thead>
            <tr>
                <th>Tên tag</th>
                <th>Hình ảnh</th>
                <th>Thời gian tham gia</th>
               
                <th>Email</th>
                <th>Ghi chú</th>
                <th>Số bài đã viết</th>
                
            </tr>
            </thead>
            <tbody>
            {AuthorList.length > 0 ? (
                AuthorList.map((item, index) => (
                    
                <tr key={index}>
                    
                    <td>{item.fullName}</td>
                    <td><img src="https://i.pravatar.cc/200"></img></td>
                    <td>{format(new Date(item.joinedDate), 'dd/mm/yyyy')}</td>
                    <td>{item.email}</td>
                    <td>{item.notes}</td>
                    <td>{item.postCount}</td>

                </tr>
                ))
            ) : (
                <tr>
                <td colSpan={4}>
                    <h4 className="text-danger text-center">
                    Không tìm thấy tag nào
                    </h4>
                </td>
                </tr>
            )}
            </tbody>
     </Table>
   </>
  );
};
export default AuthorsList;
