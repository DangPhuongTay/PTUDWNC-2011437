import { useState, useEffect } from "react";
import ListGroup from "react-bootstrap/ListGroup";
import { Link } from "react-router-dom";
import Table from "react-bootstrap/Table";
import { getCategories } from "../../Services/Widgets";


const CategoriesWidget = () => {
  const [categoryList, setCategoryList] = useState([]);

  useEffect(() => {
    getCategories().then((data) => {
      if (data) setCategoryList(data);
      else setCategoryList([]);
    });
  }, []);
  
  return (
    <>
     <Table striped responsive bordered>
           
            <thead>
            <tr>
                <th>Tên chủ đề</th>
                <th>Mô tả</th>
                <th>Số bài viết thuộc chủ đề</th>
                
            </tr>
            </thead>
            <tbody>
            {categoryList.length > 0 ? (
                categoryList.map((item, index) => (
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
                    Không tìm thấy bài viết nào
                    </h4>
                </td>
                </tr>
            )}
            </tbody>
     </Table>
   </>
  );
};
export default CategoriesWidget;
