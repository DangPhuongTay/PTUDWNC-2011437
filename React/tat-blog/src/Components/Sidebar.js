import React from 'react';
import SearchForm from './SearchForm';
import CategoriesWidget from './CategoriesWidget';
import FeaturtePostsWidget from './FeaturtePostsWidget';
import RandomPostsWidget from './RandomPostsWidget';
const Sidebar = () => {
  return (
    <div className="pt-4 ps-2">
      <h1>Tìm kiếm bài viết</h1>
      <SearchForm/>

      <CategoriesWidget/>
      
      <FeaturtePostsWidget/>
      <RandomPostsWidget/>
      <h1>Tag cloud</h1>
    </div>
  );
};
export default Sidebar;
