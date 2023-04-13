import axios from "axios";
export async function getCategories() {
  try {
    const response = await axios.get("https://localhost:7219/api/categories");
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log("Error", error.message);
    return null;
  }
}
export async function getAuthors() {
  try {
    const response = await axios.get("https://localhost:7219/api/authors");
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log("Error", error.message);
    return null;
  }
}
export async function getTags() {
  try {
    const response = await axios.get("https://localhost:7219/api/tags");
    const data = response.data;
    if (data.isSuccess) return data.result;
    else return null;
  } catch (error) {
    console.log("Error", error.message);
    return null;
  }
}
export async function getFeaturedPosts(limit){
  try{
      const response = await axios.get(`https://localhost:7219/api/posts/featured/${limit}`);

      const data = response.data;
      if (data.isSuccess)           
          return data.result;
          else
          return null;

  } catch (error) {
      console.log('Error', error.message);
      return null;
  }
}
export async function getRandomPosts(limit){
  try{
      const response = await axios.get(`https://localhost:7219/api/posts/random/${limit}`);

      const data = response.data;
      if (data.isSuccess)           
          return data.result;
          else
          return null;

  } catch (error) {
      console.log('Error', error.message);
      return null;
  }
}
