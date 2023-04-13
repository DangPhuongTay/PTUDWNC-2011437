import { useState, useEffect } from "react";

import Table from "react-bootstrap/Table";
import { getTags } from "../../Services/Widgets";


const TagsList = () => {
  const [TagsList, setTagList] = useState([]);

  useEffect(() => {
    getTags().then((data) => {
      if (data) setTagList(data);
      else setTagList([]);
    });
  }, []);
  
  return (
    <>
     <Table striped responsive bordered>
           
            <thead>
            <tr>
                <th>Tên tag</th>
                <th>Mô tả</th>
                <th>Số bài viết thuộc tag</th>
                
            </tr>
            </thead>
            <tbody>
            {TagsList.length > 0 ? (
                TagsList.map((item, index) => (
                <tr key={index}>

                    <td>{item.name}</td>
                    <td>{item.description}</td>
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
export default TagsList;
