import axios from "axios";
export async function getPosts(
  keyword = "",
  pageSize = 2,
  pageNumber = 1,
  sortColumn = "",
  sortOrder = ""
) {
  try {
    const response = await axios.get(
      `https://localhost:7219/api/posts?keyword=${keyword}&PageSize=${pageSize}&PageNumber=${pageNumber}&SortColumn=${sortColumn}&SortOrder=${sortOrder}`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log("Error", error.message);
    return null;
  }
}
export async function getPost(
  year = 2023,
  month = 1,
  day = 1,
  slug = '',
) {
  try {
    const response = await axios.get(
      `https://localhost:7129/api/posts?PageSize=1&PageNumber=1&Year=${year}&Month=${month}&Day=${day}&PostSlug=${slug}`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}

export async function getPostBySlug(slug = '') {
  try {
    const response = await axios.get(
      `https://localhost:7129/api/posts/byslug/${slug}?PageSize=2&PageNumber=1`
    );
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log('Error', error.message);
    return null;
  }
}