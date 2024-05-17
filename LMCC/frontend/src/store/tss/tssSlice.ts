import { createSlice } from '@reduxjs/toolkit';

const tssSlice = createSlice({
  name: 'tss',
  initialState: {
    uia: null,
    dcu: null,
    rover: null,
  },
  reducers: {
    updateTSS: (state, action) => {
      const { uia, dcu, rover } = action.payload;
      state.uia = uia;
      state.dcu = dcu;
      state.rover = rover;
    },
  },
});

export const { updateTSS } = tssSlice.actions;
export default tssSlice;
