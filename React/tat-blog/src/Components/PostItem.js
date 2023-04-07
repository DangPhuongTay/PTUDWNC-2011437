import TagList from "./TagList";
import Card from "react-bootstrap/Card";
import { Link } from "react-router-dom";
import { isEmptyOrSpaces } from "../Utils/Utils";

const PostList = ({ postItem }) => {
  let imageUrl = isEmptyOrSpaces(postItem.imageUrl)
    ? process.env.PUBLIC_URL + "/images/image_1.png"
    : `${postItem.imageUrl}`;
  let postedDate = new Date(postItem.postedDate);
  return (
    <article className="blog-entry mb-4">
      <Card>
        <div className="row g-0">
          <div className="col-md-4">
            <Card.Img variant="top" src={imageUrl} alt={postItem.title} />
          </div>
          <div className="col-md-8">
            <Card.Body>
              <Card.Title>{postItem.title}</Card.Title>
              <Card.Text>
                <small className="text-muted">Tác giả:</small>
                <Link to={`/author/${postItem.author.urlSlug}`}>
                <span className="text-primary m-1">
                {postItem.author.fullName}
                </span>
                </Link>
                <small className="text-muted">Chủ đề:</small>
                <Link to={`/author/${postItem.category.urlSlug}`}>
                <span className="text-primary m-1">
                  {postItem.category.name}
                </span>
                </Link>
              </Card.Text>
              
              <Card.Text>{postItem.shortDescription}</Card.Text>
              <div className="tag-list">
                <TagList tagList={postItem.tags} />
              </div>
              <div className="text-end">
                <Link
                  to={`/blog/post?year=${postedDate.getFullYear()}&month=${postedDate.getMonth()}&day=${postedDate.getDay()}&slug=${
                    postItem.urlSlug
                  }`}
                  className="btn btn-primary"
                  title={postItem.title}
                >
                  Xem chi tiết
                </Link>
              </div>
            </Card.Body>
          </div>
        </div>
      </Card>
    </article>
  );
};
export default PostList;

// import TagList from './TagList';
// import Card from 'react-bootstrap/Card';
// import { Link } from 'react-router-dom';
// import { isEmptyOrSpaces } from '../Utils/Utils';

// const PostList = ({ postItem }) => {
//   let imageUrl = isEmptyOrSpaces(postItem.imageUrl)
//     ? import.meta.env.VITE_PUBLIC_URL + '/images/image_1.jpg'
//     : `${postItem.imageUrl}`;

//   let postedDate = new Date(postItem.postedDate);

//   return (
//     <article className="blog-entry  mb-4">
//       <Card>
//         <div className="row  g-0">
//           <div className="col-md-4">
//             <Card.Img variant="top" src={imageUrl} alt={postItem.title} />
//           </div>
//           <div className="col-md-8">
//             <Card.Body>
//               <Card.Title>
//                 <Link
//                   to={`/blog/post/${postedDate.getFullYear()}/${postedDate.getMonth() + 1}/${postedDate.getDate()}/${
//                     postItem.urlSlug
//                   }`}
//                   style={{ textDecoration: 'none' }}
//                   title={postItem.title}
//                 >
//                   {postItem.title}
//                 </Link>
//               </Card.Title>
//               <Card.Text>
//                 <small className="text-muted">Tác giả:</small>
//                 <span className="text-primary  m-1">
//                   <Link
//                     to={`/blog/author/${postItem.author.urlSlug}`}
//                     style={{ textDecoration: 'none' }}
//                     title={postItem.author.fullName}
//                   >
//                     {postItem.author.fullName}
//                   </Link>
//                 </span>
//                 <small className="text-muted">Chủ đề:</small>
//                 <span className="text-primary  m-1">
//                   <Link
//                     to={`/blog/category/${postItem.category.urlSlug}`}
//                     style={{ textDecoration: 'none' }}
//                     title={postItem.category.name}
//                   >
//                     {postItem.category.name}
//                   </Link>
//                 </span>
//               </Card.Text>
//               <Card.Text>{postItem.shortDescription}</Card.Text>
//               <div className="tag-list">
//                 <TagList tagList={postItem.tags} />
//               </div>
//               <div className="text-end">
//                 <Link
//                   to={`/blog/post?year=${postedDate.getFullYear()}&month=${postedDate.getMonth() + 1}&day=${postedDate.getDate()}&slug=${
//                     postItem.urlSlug
//                   }`}
//                   className="btn  btn-primary"
//                   title={postItem.title}
//                 >
//                   Xem chi tiết
//                 </Link>
//               </div>
//             </Card.Body>
//           </div>
//         </div>
//       </Card>
//     </article>
//   );
// };

// export default PostList;
