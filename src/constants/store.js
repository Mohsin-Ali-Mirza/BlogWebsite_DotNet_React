import { configureStore } from '@reduxjs/toolkit';
import { appApi } from '../features/api.slice';
import user from '../features/user.slice';

const store = configureStore({
    reducer: {
        user,
        [appApi.reducerPath]: appApi.reducer
    },
    middleware: (getDefaultMiddleware) => getDefaultMiddleware()
        .concat(appApi.middleware)
});

export default store;